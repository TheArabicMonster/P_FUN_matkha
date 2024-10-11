using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiftMetrics
{
    public class RegionData
    {
        public Dictionary<string, string> Regions { get; set; }
        public Dictionary<string, List<string>> RegionClusters { get; set; }
        public RegionData()
        {
            Regions = new Dictionary<string, string>();
            RegionClusters = new Dictionary<string, List<string>>();
            AjoutRegion();
        }

        /// <summary>
        /// Ajoute des régions et des clusters de régions à la liste
        /// </summary>
        private void AjoutRegion()
        {
            // Ajouter des régions
            Regions.Add("NA", "North America");
            Regions.Add("EUW", "Europe West");
            Regions.Add("EUNE", "Europe Nordic & East");
            Regions.Add("OCE", "Oceania");
            Regions.Add("RU", "Russia");
            Regions.Add("TR", "Turkey");
            Regions.Add("BR", "Brazil");
            Regions.Add("LAN", "Latin America North");
            Regions.Add("LAS", "Latin America South");
            Regions.Add("JP", "Japan");
            Regions.Add("TW", "Taiwan");
            Regions.Add("SGMYID", "Singapore, Malaysia, Indonesia");
            Regions.Add("TH", "Thailand");
            Regions.Add("PH", "Philippines");
            Regions.Add("ME", "Middle East");

            // Ajouter des clusters de régions
            RegionClusters.Add("Americas", ["NA", "BR", "LAN", "LAS"]);
            RegionClusters.Add("Europe", ["EUW", "EUNE", "TR", "RU"]);
            RegionClusters.Add("Asia", ["JP", "TW", "SGMYID", "TH", "PH"]);
            RegionClusters.Add("Oceania", ["OCE"]);
            RegionClusters.Add("Middle East", ["ME"]);
        }

        /// <summary>
        /// retourne le clusture qui contient la region donnée
        /// </summary>
        /// <param name="region">region choisis par utilisateur (exemple: europe west)</param>
        /// <returns></returns>
        public string ClusterDepuisRegion(string region)
        {
            string regionCode = RegionCodeDepuisNom(region);
            return RegionClusters.FirstOrDefault(c => c.Value.Contains(regionCode)).Key ?? "unkownCluster";
        }

        /// <summary>
        /// Retourne le code de la region qui corresponds au nom de la region donné en paramètre
        /// </summary>
        /// <param name="region">nom de la région dont on souhaite trouvé le code (exemple: Europe West)</param>
        /// <returns></returns>
        private string RegionCodeDepuisNom(string region)
        {
            return Regions.FirstOrDefault(r => r.Value == region).Key  ?? "unkownCluster";
        }
    }
}
