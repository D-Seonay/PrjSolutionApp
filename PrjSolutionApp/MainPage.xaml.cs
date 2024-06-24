using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace YourNamespace
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnRechercherClicked(object sender, EventArgs e)
        {
            string prenom = PrenomEntry?.Text;
            string annee = AnneeEntry?.Text;

            if (string.IsNullOrEmpty(prenom) || string.IsNullOrEmpty(annee))
            {
                await DisplayAlert("Erreur", "Veuillez entrer un prénom et une année", "OK");
                return;
            }

            try
            {
                if (!int.TryParse(annee, out int anneeInt))
                {
                    await DisplayAlert("Erreur", "L'année doit être un nombre valide", "OK");
                    return;
                }

                string result = await GetNombreEnfantsNes(prenom, anneeInt);
                if (result != null)
                {
                    ResultLabel.Text = $"Nombre d'enfants nés en {annee} avec le prénom {prenom}: {result}";
                }
                else
                {
                    await DisplayAlert("Erreur", "Aucun résultat trouvé", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erreur", "Une erreur est survenue: " + ex.Message, "OK");
            }
        }

        private async Task<string> GetNombreEnfantsNes(string prenom, int annee)
        {
            string apiUrl = $"https://data.nantesmetropole.fr/api/explore/v2.1/catalog/datasets/244400404_prenoms-enfants-nes-nantes/records?limit=20&refine=enfant_prenom%3A%22{prenom}%22&refine=annee%3A%22{annee}%22";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                string responseBody = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(responseBody);

                // Extraire le nombre d'enfants nés de la réponse JSON
                int nombreEnfants = json["total_count"]?.ToObject<int>() ?? 0;
                return nombreEnfants.ToString();
            }
        }
    }
}
