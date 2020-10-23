using CafeBoost.Data;
using CafeBoost.UI.Properties;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CafeBoost.UI
{
    public partial class AnaForm : Form
    {
        //int masaAdet = 20;
        KafeVeri db;

        public AnaForm()
        {
            VeriOku();
            InitializeComponent();
            OrnekUrunleriYukle();
            MasalariOlustur();
        }


        private void OrnekUrunleriYukle()
        {
            db.Urunler.Add(new Urun
            {
                UrunAd = "Kola",
                BirimFiyat = 6m

            });
            db.Urunler.Add(new Urun
            {
                UrunAd = "Ayran",
                BirimFiyat = 4m

            });

        }

        private void MasalariOlustur()
        {
            #region İmaj Listesinin Hazırlanması
            ImageList il = new ImageList();
            il.Images.Add("bos", Resources.BosMasa);
            il.Images.Add("dolu", Resources.DoluMasa);
            il.ImageSize = new Size(64, 64);
            lvwMasalar.LargeImageList = il;

            #endregion

            #region Masaların Oluşturulması

            ListViewItem lvi;
            for (int i = 1; i <= db.MasaAdet; i++)
            {
                lvi = new ListViewItem("Masa" + i);
                //lvi.ImageKey = "bos";
                lvi.ImageKey = db.AktifSiparisler.Any(x => x.MasaNo == i) ? "dolu" : "bos";
                lvi.Tag = i;
                lvwMasalar.Items.Add(lvi);


            }
            #endregion
        }
        private void tsmiGecmisSiparisler_Click(object sender, EventArgs e)
        {
            new GecmisSiparislerForm(db).ShowDialog();
        }

        private void lvwMasalar_DoubleClick(object sender, EventArgs e)
        {
            int masaNo = (int)lvwMasalar.SelectedItems[0].Tag;

            // bu masa no ile ne yapacağız 
            // ya bu masa no ya ait sipariş vardır || yoktur.
            //varsa olan siparişi bul getir.
            //yoksa yeni sipariş yarat.
            //yeni yaratılanı || olanı yeni SiparisDetaylar formuna göndererek aç

            Siparis siparis = AktifSiparisBul(masaNo);

            if (siparis == null)
            {
                siparis = new Siparis();
                siparis.MasaNo = masaNo;
                db.AktifSiparisler.Add(siparis);
                lvwMasalar.SelectedItems[0].ImageKey = "dolu";
            }

            SiparisForm frmSiparis = new SiparisForm(db, siparis, this);
            DialogResult dr = frmSiparis.ShowDialog();

            // new SiparisForm(db, siparis).ShowDialog();

            // Sipariş iptal edildiyse ya da ödeme alındıysa
            if (dr == DialogResult.OK)
            {
                lvwMasalar.SelectedItems[0].ImageKey = "bos";
            }
        }

        private Siparis AktifSiparisBul(int masaNo)
        {
            return db.AktifSiparisler.FirstOrDefault(x => x.MasaNo == masaNo); // x her bir siparişi temsil ediyor. metodu parametre olarak soktuk delegeler sayesinde.

            #region Foreach Yöntemi
            //foreach (var item in db.AktifSiparisler)
            //{
            //    if (item.MasaNo == masaNo)
            //    {
            //        return item;
            //    }

            //}
            //return null; 
            #endregion


        }

        public void MasaTasi(int kaynak, int hedef)
        {
            foreach (ListViewItem lvi in lvwMasalar.Items)
            {
                if ((int)lvi.Tag == kaynak)
                {
                    lvi.ImageKey = "bos";
                }
                if ((int)lvi.Tag == hedef)
                {
                    lvi.ImageKey = "dolu";
                }

            }
        }
        private void lvwMasalar_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tsmiUrunler_Click(object sender, EventArgs e)
        {
            new UrunlerForm(db).ShowDialog();
        }

        private void AnaForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            VeriKaydet();
        }
        private void VeriOku()
        {
            try
            {
                string json = File.ReadAllText("veri.json");
                db = JsonConvert.DeserializeObject<KafeVeri>(json);
            }
            catch (Exception)
            {
                db = new KafeVeri();
            }
        }

        private void VeriKaydet()
        {
            string json = JsonConvert.SerializeObject(db, Formatting.Indented);
            File.WriteAllText("veri.json", json);
        }
    }
}
