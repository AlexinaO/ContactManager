using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ContactsManager.Metier;

namespace ContactsManager
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Contact> liste = RecupererListeContactsDepuisFichier();

            while (true)
            {
                AfficherMenu();

                string saisie = Console.ReadLine();
                switch (saisie.ToUpper())
                {
                    case "1":
                        ListerContacts(liste);
                        break;
                    case "2":
                        AjouterContact(liste);
                        break;
                    case "3":
                        SupprimerContact(liste);
                        break;
                    case "4":
                        TrierContacts(liste);
                        break;
                    case "5":
                        ChercherContacts(liste);
                        break;
                    case "Q":
                        EnregistrerListeContactsDansFichier(liste);
                        return;
                }




                AfficherLignePourRetournerAuMenu();
            }
        }

        private static List<Contact> RecupererListeContactsDepuisFichier()
        {
            var liste = new List<Contact>();

            if (File.Exists("Contacts.txt"))
            {
                var lignes = File.ReadAllLines("Contacts.txt");

                foreach (var ligne in lignes)
                {
                    string[] champs = ligne.Split(';');
                    var contact = new Contact
                    {
                        Nom = champs[0],
                        Prenom = champs[1],
                        Email = champs[2]
                    };

                    liste.Add(contact);
                }
            }

            return liste;
        }

        private static void EnregistrerListeContactsDansFichier(List<Contact> liste)
        {
            var stringBuilder = new StringBuilder();
            foreach(var contact in liste)
            {
                stringBuilder.AppendLine(
                    string.Join(";", contact.Nom, contact.Prenom, contact.Email));
            }

            File.WriteAllText("Contacts.txt", stringBuilder.ToString());
        }

        private static void ChercherContacts(List<Contact> liste)
        {
            AfficherEntete("Chercher des contacts");

            Console.Write("Entrez le début du nom ou du prénom : ");
            var debutNomOuPrenom = Console.ReadLine();

            var requete = from contact in liste
                          where contact.Nom.StartsWith(debutNomOuPrenom, StringComparison.OrdinalIgnoreCase)
                            || contact.Prenom.StartsWith(debutNomOuPrenom, StringComparison.OrdinalIgnoreCase)
                          orderby contact.Nom
                          select contact;

            Console.WriteLine();
            AfficherListeContacts(requete.ToList(), false);
        }

        private static void TrierContacts(List<Contact> liste)
        {
            AfficherEntete("Trier les contacts");

            Console.WriteLine("Comment souhaitez-vous trier ?");
            Console.WriteLine("1 - Par nom");
            Console.WriteLine("2 - Par prénom");
            Console.Write("Votre choix : ");
            var choix = Console.ReadLine();

            var requete = from contact in liste
                          select contact;
            switch (choix)
            {
                case "1":
                    requete = requete.OrderBy(x => x.Nom);
                    break;

                case "2":
                    requete = requete.OrderBy(x => x.Prenom);
                    break;

                default:
                    Console.WriteLine("Désolé, choix inconnu");
                    return;
            }

            Console.WriteLine();
            AfficherListeContacts(requete.ToList(), false);
        }

        private static void ListerContacts(List<Contact> liste)
        {
            AfficherEntete("Liste des contacts");
            AfficherListeContacts(liste, false);
        }

        private static void AfficherListeContacts(List<Contact> liste, bool afficherIndex)
        {
            if (liste.Count == 0)
            {
                Console.WriteLine();
                Console.WriteLine("Aucun contact pour le moment");
                return;
            }

            var lignesBuilder = new StringBuilder();

            // Construction de la ligne d'en-tête
            if (afficherIndex)
            {
                lignesBuilder.AppendFormat("{0,-5} ", "NUM");
            }
            lignesBuilder.AppendFormat("{0,-20} {1,-20} {2,-50}", "NOM", "PRENOM", "EMAIL");
            lignesBuilder.AppendLine();

            for (int i = 0; i < liste.Count; i++)
            {
                var contact = liste[i];

                if (afficherIndex)
                {
                    lignesBuilder.AppendFormat($"{i,-5} ");
                }
                lignesBuilder.Append($"{contact.Nom,-20} {contact.Prenom,-20} {contact.Email,-50}");
                lignesBuilder.AppendLine();
            }

            Console.Write(lignesBuilder.ToString());
        }

        private static void AjouterContact(List<Contact> liste)
        {
            AfficherEntete("Nouveau contact");

            Contact contact = new Contact();

            Console.Write("Entrez le nom : ");
            contact.Nom = Console.ReadLine();

            Console.Write("Entrez le prénom : ");
            contact.Prenom = Console.ReadLine();

            Console.Write("Entrez l'email : ");
            contact.Email = Console.ReadLine();

            liste.Add(contact);
        }

        private static void SupprimerContact(List<Contact> liste)
        {
            AfficherEntete("Supprimer un contact");

            AfficherListeContacts(liste, true);

            if (liste.Count > 0)
            {
                Console.Write("Entrez le numéro du contact à supprimer: ");
                var numeroContact = int.Parse(Console.ReadLine());
                liste.RemoveAt(numeroContact);
                Console.WriteLine("Contact supprimé !");
            }
        }

        private static void AfficherMenu()
        {
            AfficherEntete("MENU");

            Console.WriteLine("1. Liste des contacts");
            Console.WriteLine("2. Ajouter un contact");
            Console.WriteLine("3. Supprimer un contact");
            Console.WriteLine("4. Trier contacts");
            Console.WriteLine("5. Chercher des contacts");
            Console.WriteLine("Q. Quitter");
            Console.WriteLine();
            Console.Write("Votre choix: ");
        }

        private static void AfficherEntete(string libelle)
        {
            Console.Clear();

            // Comme j'utilise "| " et " |", la ligne doit avoir 4 caractères en plus
            var ligneTraits = new string('*', libelle.Length + 4);
            Console.WriteLine(ligneTraits);
            Console.WriteLine($"| { libelle } |");
            Console.WriteLine(ligneTraits);
        }

        private static void AfficherLignePourRetournerAuMenu()
        {
            Console.WriteLine();
            Console.Write("> Appuyez pour retourner au menu...");
            Console.ReadKey();
        }
    }
}
