using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;


namespace test_vooban.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult countNamesWithAtLeastTwoVowels(HttpPostedFileBase fileInput)
        {
            Char[] vowels = { 'A', 'E', 'I', 'O', 'U', 'Y' };
            int vowelCount = 0;
            List<string> lstNames = new List<string>();
            string nameEntry;

            if (fileInput != null && fileInput.ContentLength > 0)
            {
                StreamReader sr = new StreamReader(fileInput.InputStream);
                while (!sr.EndOfStream)
                {
                    nameEntry = sr.ReadLine();
                    foreach (char c in nameEntry)
                    {
                        if (vowels.Contains(c))
                        {
                            vowelCount++;
                        }
                    }
                    if (vowelCount >= 2)
                    {
                        lstNames.Add(nameEntry);
                    }
                    vowelCount = 0;
                }
                sr.Close();

                if (lstNames.Count() > 0)
                {
                    string test = Server.MapPath("~");
                    StreamWriter sw = new StreamWriter(Server.MapPath("~") + @"\sortedNames.txt");
                    foreach (string line in lstNames)
                    {
                        sw.WriteLine(line);
                    }
                    sw.Close();
                }
                TempData["FirstMessage"] = String.Format("{0} noms ont été enregistrés.", lstNames.Count);
            }
            else
            {
                TempData["FirstMessage"] = "Aucun fichier selectionné.";
            }

            return RedirectToAction("Index");
        }

        public ActionResult downloadFile()
        {
            FileContentResult toDownload;
            string filePath = Server.MapPath("~") + @"\sortedNames.txt";

            if (System.IO.File.Exists(filePath))
            {
                toDownload = new FileContentResult(System.IO.File.ReadAllBytes(filePath), "application/msword")
                {
                    FileDownloadName = Path.GetFileName(filePath)
                };
                return toDownload;
            }
            else
            {
                TempData["downloadError"] = "Le fichier doit être traité avant d'être téléchargé";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult calculateScore(HttpPostedFileBase fileInput)
        {
            bool negativeScoreReached = false;
            char inputCharacter;
            int negativeScoreIndex = 0, index = 0, score = 0;

            if (fileInput != null && fileInput.ContentLength > 0)
            {
                StreamReader sr = new StreamReader(fileInput.InputStream);
                while (!sr.EndOfStream)
                {
                    inputCharacter = (char)sr.Read();
                    if (inputCharacter == '(')
                    {
                        score++;
                    }
                    else if (inputCharacter == ')')
                    {
                        score--;
                    }

                    if (!negativeScoreReached && score == -1)
                    {
                        negativeScoreReached = true;
                        negativeScoreIndex = index;
                    }
                    index++;
                }
                TempData["SecondMessage"] = String.Format("Score final : {0}. Position du premier score négatif : {1}", score, negativeScoreIndex > 0 ? negativeScoreIndex.ToString() : "Aucun");
            }
            else
            {
                TempData["SecondMessage"] = "Aucun fichier selectionné.";
            }

            return RedirectToAction("Index");
        }
    }
}