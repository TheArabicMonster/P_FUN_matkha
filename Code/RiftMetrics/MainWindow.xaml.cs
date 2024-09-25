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

namespace RiftMetrics
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> Regions { get; set; } ///Liste des régions
        public RegionData RegionData { get; set; } ///Données des régions
        public PlotModel PlotModel { get; set; } ///Modèle de graphique OxyPlot
        private Invocateur invocateur = new Invocateur(); ///Invocateur

        private readonly string apiKeyRiot = "RGAPI-ccd67026-76de-4a1e-b146-bc109e90c9c8"; ///Clé API de Riot Games

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            RegionData = new RegionData();
            Regions = new ObservableCollection<string>(RegionData.Regions.Values);
            RegionComboBox.ItemsSource = Regions;

            {
                PlotModel = new PlotModel { Title = "Test graphique OxyPlot" };

                var lineSeries = new LineSeries
                {
                    Title = "Courbe 1",
                    MarkerType = MarkerType.Circle,
                };

                lineSeries.Points.Add(new DataPoint(0, 0));
                lineSeries.Points.Add(new DataPoint(10, 18));
                lineSeries.Points.Add(new DataPoint(20, 12));
                lineSeries.Points.Add(new DataPoint(30, 8));
                lineSeries.Points.Add(new DataPoint(40, 15));



                PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Temps" });
                PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Nombre de joueurs" });
                PlotModel.Series.Add(lineSeries);

                var secondLineSeries = new LineSeries
                {
                    Title = "Courbe 2",
                    MarkerType = MarkerType.Square
                };
                secondLineSeries.Points.Add(new DataPoint(0, 5));
                secondLineSeries.Points.Add(new DataPoint(10, 10));
                secondLineSeries.Points.Add(new DataPoint(20, 6));
                secondLineSeries.Points.Add(new DataPoint(30, 14));
                secondLineSeries.Points.Add(new DataPoint(40, 9));

                PlotModel.Series.Add(secondLineSeries);
            }
        }

        /// <summary>
        /// Méthode appelée lors de la sélection d'une région dans la ComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(RegionComboBox.SelectedItem != null)
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

        }
    }
}