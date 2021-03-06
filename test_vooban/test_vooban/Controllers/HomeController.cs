﻿using System;
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
                try
                {
                    using (StreamReader sr = new StreamReader(fileInput.InputStream))
                    {
                        while (!sr.EndOfStream)
                        {
                            nameEntry = sr.ReadLine();
                            foreach (char c in nameEntry)
                            {
                                if (vowels.Contains(Char.ToUpper(c)))
                                {
                                    vowelCount++;
                                }
                            }
                            if (vowelCount % 2 == 0)
                            {
                                lstNames.Add(nameEntry);
                            }
                            vowelCount = 0;
                        }
                    }

                    if (lstNames.Count() > 0)
                    {
                        using (StreamWriter sw = new StreamWriter(Server.MapPath("~") + @"\sortedNames.txt"))
                        {
                            lstNames.Sort();

                            foreach (string line in lstNames)
                            {
                                sw.WriteLine(line);
                            }
                        }
                        TempData["FirstMessage"] = String.Format("{0} noms ont été enregistrés.", lstNames.Count);
                    }
                }
                catch(Exception e)
                {
                    TempData["FirstMessage"] = "Erreur dans la lecture ou l'écriture du fichier";
                }
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
                try
                {
                    using (StreamReader sr = new StreamReader(fileInput.InputStream))
                    {
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
                    }
                    TempData["SecondMessage"] = String.Format("Score final : {0}. Position du premier score négatif : {1}", score, negativeScoreIndex > 0 ? negativeScoreIndex.ToString() : "Aucun");
                }
                catch(Exception e)
                {
                    TempData["SecondMessage"] = "Erreur lors de l'ouverture du fichier";
                }
            }
            else
            {
                TempData["SecondMessage"] = "Aucun fichier selectionné.";
            }
            return RedirectToAction("Index");
        }
    }
}