using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Net.Http;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;
using OxyPlot.Wpf;

namespace RiftMetrics
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> Regions { get; set; } ///Liste des régions
        public RegionData RegionData { get; set; } ///Données des régions
        public PlotModel PlotModel { get; set; } ///Modèle de graphique OxyPlot
        public PlotModel PlotModelFromJson { get; set; }
        private Invocateur invocateur = new Invocateur(); ///Invocateur
        private readonly string apiKeyRiot = "RGAPI-ccd67026-76de-4a1e-b146-bc109e90c9c8"; ///Clé API de Riot Games

        //TODO -> mettre toutes ces class dans un fichier a par 
        public class RootObject
        {
            public string Timestamp { get; set; }
            public List<GameData> Games { get; set; }
        }
        public class GameData
        {
            public int AppId { get; set; }
            public string Name { get; set; }
            public int CurrentPlayers { get; set; }
            public int Peak24hPlayers { get; set; }
            public int PeakWeeklyPlayers { get; set; }
            public int PeakMonthlyPlayers { get; set; }

            [JsonProperty("player_trends")]
            public PlayerTrends PlayerTrends { get; set; }
        }
        public class PlayerTrends
        {
            public List<HourlyTrend> Hourly { get; set; }
            public List<DailyTrend> Daily { get; set; }
            public List<WeeklyTrend> Weekly { get; set; }
        }
        public class HourlyTrend
        {
            public string Day { get; set; }
            public string Hour { get; set; }
            public int Players { get; set; }
        }
        public class DailyTrend
        {
            public string Day { get; set; }
            public int Players { get; set; }
        }
        public class WeeklyTrend
        {
            public string Week { get; set; }
            public int Players { get; set; }
        }


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            RegionData = new RegionData();
            Regions = new ObservableCollection<string>(RegionData.Regions.Values);
            RegionComboBox.ItemsSource = Regions;

            ChargerTempsTop3JeuSteam("hour");
        }

        /// <summary>
        /// Méthode appelée lors de la sélection d'une région dans la ComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RegionComboBox.SelectedItem != null)
            {
                invocateur.Region = RegionComboBox.SelectedItem.ToString();
                Debug.WriteLine(invocateur.Region);
            }
        }

        /// <summary>
        /// Méthode appelée lors de la modification du texte dans la TextBox du nom de l'invocateur
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NomInvocateur_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null)
            {
                string text = textBox.Text;

                int index = text.IndexOf('#');

                if (index >= 0)
                {
                    invocateur.Nom = text.Substring(0, index);
                    Debug.WriteLine("Nom de l'invocateur : " + invocateur.Nom);
                    invocateur.Id = text.Substring(index);
                    Debug.WriteLine("Id de l'invocateur : " + invocateur.Id);
                }
                else
                {
                    invocateur.Nom = text;
                    invocateur.Id = string.Empty;
                }
            }
        }

        /// <summary>
        /// Recherche les informations d'un invocateur sur l'API de Riot Games
        /// getSummonerByName
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Boutton_Recherche_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(invocateur.Region) || string.IsNullOrEmpty(invocateur.Nom))
            {
                MessageBox.Show("Entrez un nom d'invocateur et une region");
                return;
            }
            else
            {

                Debug.WriteLine("Recherche de l'invocateur " + invocateur.Nom + " avec l'id " + invocateur.Id + " dans la région " + invocateur.Region + " dans le cluster " + invocateur.Clusters);
            }

        }
        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            // Logique à exécuter lorsque l'un des boutons avec images est cliqué
            MessageBox.Show("Bouton image cliqué !");
        }

        private void TimeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TimeComboBox.SelectedItem != null)
            {
                string selectedTimeFrame = (TimeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

                // Effacer les séries existantes avant de redessiner
                PlotModel.Series.Clear();

                // Charger les données en fonction de la sélection
                ChargerTempsTop3JeuSteam(selectedTimeFrame);
                plotView.Model = PlotModel;
                PlotModel.InvalidatePlot(true);

            }
        }

        private void ChargerTempsTop3JeuSteam(string timeFrame)
        {
            try
            {
                string json = File.ReadAllText("extended_player_data_top_3_steam_games.json");
                var rootObject = JsonConvert.DeserializeObject<RootObject>(json); // Désérialiser en RootObject

                PlotModelFromJson = new PlotModel { Title = $"Number of players per {timeFrame}" };

                var dateTimeAxis = new DateTimeAxis
                {
                    Position = AxisPosition.Bottom,
                    StringFormat = "HH:mm",
                    Title = "Heure",
                    IntervalType = DateTimeIntervalType.Hours,
                    MinorIntervalType = DateTimeIntervalType.Minutes,
                };

                if (timeFrame == "hour")
                {
                    dateTimeAxis.StringFormat = "HH:mm";
                    dateTimeAxis.IntervalType = DateTimeIntervalType.Hours;
                    dateTimeAxis.MinorIntervalType = DateTimeIntervalType.Minutes;
                }
                else if (timeFrame == "day")
                {
                    dateTimeAxis.StringFormat = "dd MMM";
                    dateTimeAxis.IntervalType = DateTimeIntervalType.Days;
                }
                //else if(timeFrame == "week")
                //{
                //    dateTimeAxis.StringFormat = "dd MM";
                //    dateTimeAxis.IntervalType = DateTimeIntervalType.Weeks;
                //}
                PlotModelFromJson.Axes.Add(dateTimeAxis);

                var valueAxis = new LinearAxis
                {
                    Position = AxisPosition.Left,
                    Title = "Nombre de joueurs",
                };
                PlotModelFromJson.Axes.Add(valueAxis);


                foreach (var game in rootObject.Games)
                {

                    var areaSeries = new AreaSeries
                    {
                        Title = game.Name,
                        Fill = OxyColor.FromArgb(120, 0, 255, 0), // Remplissage sous la courbe (optionnel)
                        Color = OxyColors.Green // Couleur de la ligne
                    };

                    if (timeFrame == "hour")
                    {
                        foreach (var trend in game.PlayerTrends.Hourly)
                        {
                            var dateTime = DateTime.Parse($"{trend.Day} {trend.Hour}");
                            areaSeries.Points.Add(new DataPoint(DateTime.Parse($"{trend.Day} {trend.Hour}").ToOADate(), trend.Players));
                        }
                    }
                    else if (timeFrame == "day")
                    {
                        foreach (var trend in game.PlayerTrends.Daily)
                        {
                            var dateTime = DateTime.Parse(trend.Day);
                            areaSeries.Points.Add(new DataPoint(DateTime.Parse(trend.Day).ToOADate(), trend.Players));
                        }
                    }
                    //else if (timeFrame == "week")
                    //{
                    //    foreach (var trend in game.PlayerTrends.Weekly)
                    //    {
                    //        var dateTime = DateTime.Parse($"1 {trend.Week} 1");
                    //        areaSeries.Points.Add(new DataPoint(DateTime.Parse($"1 {trend.Week} 1").ToOADate(), trend.Players));
                    //    }
                    //}

                    PlotModelFromJson.Series.Add(areaSeries);
                }

                PlotModel = PlotModelFromJson;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des données: {ex.Message}");
            }
        }

        private async void Get10DerniersMatchs()
        {
            try
            {
                string url = $"https://{invocateur.Region}.api.riotgames.com/lol/match/v4/matchlists/by-account/{invocateur.Id}?api_key={apiKeyRiot}";
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(responseBody);
                }
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("\nException Caught!");
                Debug.WriteLine("Message :{0} ", e.Message);
            }
        }
    }
}