using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace jegyvasarloAlkalmazas
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Betoltes();
        }

        private void btnVasarlas_Click(object sender, RoutedEventArgs e)
        {
            lblHiba.Visibility = Visibility.Collapsed;
            txtOsszesites.Text = "";

            if (string.IsNullOrWhiteSpace(txtNev.Text))
            {
                lblHiba.Content = "Add meg a neved!";
                lblHiba.Visibility = Visibility.Visible;
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                lblHiba.Content = "Add meg az email címed!";
                lblHiba.Visibility = Visibility.Visible;
                return;
            }

            if (!txtEmail.Text.Contains("@"))
            {
                lblHiba.Content = "Hibás email cím!";
                lblHiba.Visibility = Visibility.Visible;
                return;
            }

            if (string.IsNullOrWhiteSpace(txtJegySzam.Text))
            {
                lblHiba.Content = "Add meg a jegyek számát!";
                lblHiba.Visibility = Visibility.Visible;
                return;
            }

            int darab;
            if (!int.TryParse(txtJegySzam.Text, out darab))
            {
                lblHiba.Content = "A jegyek száma csak szám lehet!";
                lblHiba.Visibility = Visibility.Visible;
                return;
            }

            if (darab <= 0)
            {
                lblHiba.Content = "Legalább 1 jegyet vásárolj!";
                lblHiba.Visibility = Visibility.Visible;
                return;
            }

            string nev = txtNev.Text;
            string email = txtEmail.Text;
            ComboBoxItem valasztott = (ComboBoxItem)cmbEloadas.SelectedItem;
            string eloadas = valasztott.Content.ToString();
            int ar = int.Parse(valasztott.Tag.ToString());
            int osszeg = ar * darab;

            txtOsszesites.Text = "Fizetendő: " + osszeg + " Ft";

            try
            {
                string sor = nev + ";" + email + ";" + eloadas + ";" + darab + ";" + osszeg;
                File.AppendAllText("vasarlasok.txt", sor + "\n", Encoding.UTF8);

                MessageBox.Show("Sikeres vásárlás!\n\nNév: " + nev + "\nEmail: " + email + "\nElőadás: " + eloadas + "\nJegyek: " + darab + " db\nFizetendő: " + osszeg + " Ft", "Siker");

                txtNev.Clear();
                txtEmail.Clear();
                txtJegySzam.Clear();
                cmbEloadas.SelectedIndex = 0;

                Betoltes();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba: " + ex.Message);
            }
        }

        private void btnBetoltes_Click(object sender, RoutedEventArgs e)
        {
            Betoltes();
            MessageBox.Show("Betöltve!");
        }

        private void Betoltes()
        {
            lstVasarlasok.Items.Clear();

            if (!File.Exists("vasarlasok.txt"))
            {
                lstVasarlasok.Items.Add("Még nincs vásárlás.");
                return;
            }

            string[] sorok = File.ReadAllLines("vasarlasok.txt", Encoding.UTF8);

            if (sorok.Length == 0)
            {
                lstVasarlasok.Items.Add("Még nincs vásárlás.");
                return;
            }

            lstVasarlasok.Items.Add("Név                  Email                     Előadás              Db    Összeg");
            lstVasarlasok.Items.Add("--------------------------------------------------------------------------------");

            int osszesen = 0;

            foreach (string sor in sorok)
            {
                if (sor == "")
                    continue;

                string[] darabok = sor.Split(';');

                if (darabok.Length == 5)
                {
                    string nev = darabok[0];
                    string email = darabok[1];
                    string eloadas = darabok[2];
                    string db = darabok[3];
                    int osszeg = int.Parse(darabok[4]);
                    osszesen = osszesen + osszeg;

                    if (nev.Length > 20)
                        nev = nev.Substring(0, 17) + "...";
                    if (email.Length > 25)
                        email = email.Substring(0, 22) + "...";
                    if (eloadas.Length > 20)
                        eloadas = eloadas.Substring(0, 17) + "...";

                    string kimenet = "";
                    kimenet = kimenet + nev.PadRight(20);
                    kimenet = kimenet + " " + email.PadRight(25);
                    kimenet = kimenet + " " + eloadas.PadRight(20);
                    kimenet = kimenet + " " + db.PadLeft(5);
                    kimenet = kimenet + " " + osszeg.ToString().PadLeft(10) + " Ft";

                    lstVasarlasok.Items.Add(kimenet);
                }
            }

            lstVasarlasok.Items.Add("--------------------------------------------------------------------------------");
            lstVasarlasok.Items.Add("Összes bevétel: " + osszesen + " Ft");
        }
    }
}