using System.Text.RegularExpressions;
using Client.Domain.Menu;
using HtmlAgilityPack;

// Tin Posavec

namespace Client.Application.Menu
{
    public sealed class MenuWebScraper : IMenuWebScraper
    {
        private const string JELOVNIK_URL = "https://www.scvz.unizg.hr/jelovnik-varazdin/";
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<IEnumerable<DailyMenu>> DohvatiJelovnikSWeba()
        {
            var html = await _httpClient.GetStringAsync(JELOVNIK_URL);
            var doc = new HtmlDocument();

            doc.LoadHtml(html);

            var datumi = IzvuciDatume(doc);

            if (!datumi.Any())
            {
                datumi.Add(DateTime.Today);
            }

            return ParsirajSveDane(doc, datumi);
        }

        private List<DateTime> IzvuciDatume(HtmlDocument doc)
        {
            var datumi = new List<DateTime>();

            // Traži linkove s datumima
            var links = doc.DocumentNode.SelectNodes("//a");
            if (links != null)
            {
                foreach (var link in links)
                {
                    var tekst = link.InnerText.Trim();
                    var match = Regex.Match(tekst, @"(\d{1,2})\.(\d{1,2})\.(\d{4})\.");
                    if (match.Success)
                    {
                        var dan = int.Parse(match.Groups[1].Value);
                        var mjesec = int.Parse(match.Groups[2].Value);
                        var godina = int.Parse(match.Groups[3].Value);
                        datumi.Add(new DateTime(godina, mjesec, dan));
                    }
                }
            }

            return datumi.Distinct().OrderBy(d => d).ToList();
        }

        private List<DailyMenu> ParsirajSveDane(HtmlDocument doc, List<DateTime> datumi)
        {
            var rezultat = new List<DailyMenu>();

            var h3Nodes = doc.DocumentNode.SelectNodes("//h3");
            if (h3Nodes == null)
                return rezultat;

            var rucakNodes = h3Nodes.Where(n => n.InnerText.Trim().ToLower().Contains("ručak")).ToList();
            var veceraNodes = h3Nodes.Where(n => n.InnerText.Trim().ToLower().Contains("večera")).ToList();

            for (int i = 0; i < datumi.Count; i++)
            {
                var datum = datumi[i];
                var jela = new List<Meal>();

                if (i < rucakNodes.Count)
                    jela.AddRange(ParsirajSekciju(rucakNodes[i], "Ručak"));

                if (i < veceraNodes.Count)
                    jela.AddRange(ParsirajSekciju(veceraNodes[i], "Večera"));

                if (jela.Any())
                {
                    rezultat.Add(new DailyMenu(
                        0,
                        datum,
                        (int)datum.DayOfWeek,
                        DateTime.Now,
                        jela
                    ));
                }
            }

            return rezultat;
        }

        private List<Meal> ParsirajSekciju(HtmlNode h3Node, string baznaKategorija)
        {
            var jela = new List<Meal>();
            var currentNode = h3Node.NextSibling;

            while (currentNode != null && currentNode.Name != "div" && currentNode.Name != "h3")
            {
                currentNode = currentNode.NextSibling;
            }

            if (currentNode != null && currentNode.Name == "div")
            {
                var text = HtmlEntity.DeEntitize(currentNode.InnerText).Trim();
                var novaJela = ParsirajJelaIzTeksta(text, baznaKategorija);
                jela.AddRange(novaJela);
            }

            return jela;
        }

        private List<Meal> ParsirajJelaIzTeksta(string text, string baznaKategorija)
        {
            var jela = new List<Meal>();
            var trenutnaKategorija = baznaKategorija;

            var linije = text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var linija in linije)
            {
                var cleanLine = linija.Trim();
                var cleanLineLower = cleanLine.ToLower();

                if (Regex.IsMatch(cleanLine, @"^\d+\.$"))
                {
                    trenutnaKategorija = baznaKategorija + " - Meni " + cleanLine.TrimEnd('.');
                    continue;
                }
                if (cleanLineLower == "vege")
                {
                    trenutnaKategorija = baznaKategorija + " - Vege";
                    continue;
                }
                if (cleanLineLower.Contains("jela na narudžbu"))
                {
                    trenutnaKategorija = baznaKategorija + " - Po narudžbi";
                    continue;
                }

                if (string.IsNullOrWhiteSpace(cleanLine) || cleanLine.Length < 3)
                    continue;

                if (cleanLineLower.Contains("svaki meni sadrži"))
                    continue;

                jela.Add(new Meal(0, 0, cleanLine, trenutnaKategorija));
            }

            return jela;
        }

    }
}
