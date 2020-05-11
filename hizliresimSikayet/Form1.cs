using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;                            
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hizliresimSikayet
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            /*
             "Ben hiçkimseye böbürlenerek; Sagopa Kajmer bir numaradır, benim yaptığım RAP en iyisi demedim. Zaten en iyisi benim."
             https://www.youtube.com/watch?v=tagvqsAPdmY
            */
        }
        private HttpClient client = null;
        private void button1_Click(object sender, EventArgs e)
        {
            new Form2(this).ShowDialog();
        }
        private string tokeniGetir(string fileName)
        {
            string temp = "";
            string[] tokeni_bul = File.ReadAllLines(fileName);
            for (int k = 0; k < tokeni_bul.Length; k++)
            {
                if (tokeni_bul[k].Contains("_token"))
                {
                    string[] cimbiz = tokeni_bul[k].Split(' ');
                    for (int j = 0; j < cimbiz.Length; j++)
                    {
                        string[] esittir = cimbiz[j].Split('=');
                        if (esittir[0] == "value")
                        {
                            temp = esittir[1].Replace('"'.ToString(), "").Replace(">", "");
                        }
                    }
                }
            }
            return temp;
        }
        private async Task SikayetEt(string email, string sifre, string sebep, string aciklama)
        {
            #region Login
            CookieContainer cookieJar = new CookieContainer();
            string cerezler = "";
            string token = "";
            string isim = "";
            HttpClientHandler handler = new HttpClientHandler()
            {
                CookieContainer = cookieJar
            };
            client = new HttpClient(handler, true);
            client.DefaultRequestHeaders.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            client.DefaultRequestHeaders.Add("referer", "https://hizliresim.com/");
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36");
            HttpResponseMessage resultat = await client.GetAsync("https://hizliresim.com/s/giris");
            var setCookiesForLogin = resultat.Headers.GetValues("set-cookie");
            foreach (var setCookie in setCookiesForLogin)
            {
                foreach (var cookie in setCookie.Split(';'))
                {
                    try
                    {
                        string name = cookie.Split('=')[0]; string value = cookie.Split('=')[1];
                        cookieJar.Add(new Uri("https://hizliresim.com/"),
                        new Cookie(name, value));
                        cerezler += name + "=" + value + "; ";
                    }
                    catch (Exception) { }
                }

            }
            File.WriteAllText("loginPage.html", await resultat.Content.ReadAsStringAsync());
            ///////////////
            token = tokeniGetir("loginPage.html");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            client.DefaultRequestHeaders.Add("cookie", cerezler);
            client.DefaultRequestHeaders.Add("origin", "https://hizliresim.com");
            client.DefaultRequestHeaders.Remove("referer");
            client.DefaultRequestHeaders.Add("referer", "https://hizliresim.com/s/giris");
            Dictionary<string, string> datas = new Dictionary<string, string>
             {
             { "_token", token },
             { "email", email },
                { "password",sifre}
            };
            FormUrlEncodedContent content = new FormUrlEncodedContent(datas);
            HttpResponseMessage response = await client.PostAsync("https://hizliresim.com/s/giris", content);
            string responseString = await response.Content.ReadAsStringAsync();
            File.WriteAllText("logged.html", responseString);
            cerezler = "";
            var setCookiesForSpam = response.Headers.GetValues("set-cookie");
            foreach (var setCookie in setCookiesForSpam)
            {
                foreach (var cookie in setCookie.Split(';'))
                {
                    try
                    {
                        string name = cookie.Split('=')[0]; string value = cookie.Split('=')[1];
                        cookieJar.Add(new Uri("https://hizliresim.com/"),
                        new Cookie(name, value));
                        cerezler += name + "=" + value + "; ";
                    }
                    catch (Exception) { }
                }

            }
            #endregion


            //İletişim sayfası
            client.DefaultRequestHeaders.Remove("cookie");
            client.DefaultRequestHeaders.Add("cookie", cerezler);
            HttpResponseMessage iletisim = await client.GetAsync("https://hizliresim.com/s/iletisim");
            cerezler = "";
            var iletisimCookieler = iletisim.Headers.GetValues("set-cookie");
            foreach (var setCookie in iletisimCookieler)
            {
                foreach (var cookie in setCookie.Split(';'))
                {
                    try
                    {
                        string name = cookie.Split('=')[0]; string value = cookie.Split('=')[1];
                        cookieJar.Add(new Uri("https://hizliresim.com/"),
                        new Cookie(name, value));
                        cerezler += name + "=" + value + "; ";
                    }
                    catch (Exception) { }
                }

            }
            File.WriteAllText("iletisim.html", await iletisim.Content.ReadAsStringAsync());
            //Şikayet Bildiriminiz incelenecek

            string[] isimBul = File.ReadAllLines("logged.html");
            for (int j = 0; j < isimBul.Length; j++)
            {
                if (isimBul[j].Contains("header-avatar"))
                {
                    isim = isimBul[j].Replace(textBox4.Text, "").Replace("</div>", "");
                    break;
                }
            }
            //MessageBox.Show(isim);
            token = tokeniGetir("iletisim.html");
            //MessageBox.Show(cerezler, token);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            try { client.DefaultRequestHeaders.Remove("cookie"); } catch (Exception) { }
            client.DefaultRequestHeaders.Add("cookie", cerezler);
            client.DefaultRequestHeaders.Add("origin", "https://hizliresim.com");
            client.DefaultRequestHeaders.Remove("referer");
            client.DefaultRequestHeaders.Add("referer", "https://hizliresim.com/s/iletisim");
            //MessageBox.Show(sebep); test ederken ekledim ve iyiki de eklemişim, empty geldiğini gördüm. düzelttim.
            Dictionary<string, string> datasForSpam = new Dictionary<string, string>
             {
             { "_token", token },
             { "name", isim },
                { "email",email},
                {"subject",sebep },
                { "abuse_link",""},
                {"description",aciklama }
            };
            FormUrlEncodedContent content_ = new FormUrlEncodedContent(datasForSpam);
            HttpResponseMessage response_ = await client.PostAsync("https://hizliresim.com/s/iletisim", content_);
            string responseString_ = await response_.Content.ReadAsStringAsync();
            File.WriteAllText("spammed.html", responseString_);
            //MessageBox.Show("SPAMMED!!");
            if(File.ReadAllText("spammed.html").Contains("Bildiriminiz incelenecek"))
            {
                listBox1.Items.Add("!!!SPAM İSTEĞİ GÖNDERİLDİ!!! " + email + " hesabı üzerinden gönderildi.");
                DirectoryInfo inf = new DirectoryInfo(Environment.CurrentDirectory);
                FileInfo[] f_inf = inf.GetFiles("*.html");
                foreach(FileInfo temp in f_inf) { temp.Delete(); }
            }
            await Task.Delay(10);
        }
        private async void Islem()
        {
            //https://www.youtube.com/watch?v=0j3ONCyQp4w

            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if(iptal == true) { break; }
                string emil = listView1.Items[i].Text;  //birşey denediğim için hepsini ayrı değişkenlere atadım.
                string pw = listView1.Items[i].SubItems[1].Text;
                var reason = comboBox1.Items[comboBox1.SelectedIndex].ToString();
                //MessageBox.Show(reason.ToString());
                await SikayetEt(emil, pw, reason, textBox3.Text);
                //MessageBox.Show("diğer"); TEST EDERKEN BOLCA MSGBOX KULLANDIM, KALDIRABİLİRSİNİZ.
            }
        }
        private void kaldırToolStripMenuItem_Click(object sender, EventArgs e)
        {
           if(listView1.SelectedItems.Count == 1) { listView1.SelectedItems[0].Remove(); }
        }
        bool iptal = false;
        private void button2_Click(object sender, EventArgs e)
        {
            iptal = false;
            Islem();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            iptal = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog op = new OpenFileDialog())
            {
                op.Title = "Hesapların olduğu metin belgesini seçin. HESAP|ŞİFRE şeklinde.";
                op.Multiselect = false;
                op.Filter = "Metin Belgesi (*.txt) |*.txt";
                if(op.ShowDialog() == DialogResult.OK)
                {
                    foreach(string line in File.ReadAllLines(op.FileName))
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            ListViewItem lvi = new ListViewItem(line.Split('|')[0]);
                            lvi.SubItems.Add(line.Split('|')[1]);
                            listView1.Items.Add(lvi);
                        }
                    }
                }
            }
        }
    }
}
