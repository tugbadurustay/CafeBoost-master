using CafeBoost.Data;
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
    public partial class SiparisForm : Form
    {
        private readonly KafeVeri db = new KafeVeri();
        private readonly Siparis siparis;
        private readonly AnaForm anaForm;
        private readonly BindingList<SiparisDetay> blSiparisDetaylar;

        public SiparisForm(KafeVeri kafeVeri, Siparis siparis, AnaForm anaForm)
        {
            // constructor parametresi olarak gelen bu nesneleri
            // daha sonra erişebileceğimiz field'lara aktarıyoruz.
            db = kafeVeri;
            this.siparis = siparis; // this class seviyesindeki siparişi temsil ediyor.
            this.anaForm = anaForm;
            InitializeComponent();
            dgvSiparisDetaylar.AutoGenerateColumns = false;
            //Text = siparis.MasaNo.ToString();
            MasalariListele();
            UrunleriListele();
            MasaNoGuncelle();
            OdemeTutariGuncelle();

            blSiparisDetaylar = new BindingList<SiparisDetay>(siparis.SiparisDetaylar); //binding list e ekleyince data source a da haber veriyor
            blSiparisDetaylar.ListChanged += BlSiparisDetaylar_ListChanged;
            dgvSiparisDetaylar.DataSource = blSiparisDetaylar;


        }

        private void MasalariListele()
        {
            cboMasalar.Items.Clear();

            for (int i = 1; i <= db.MasaAdet; i++)
            {
                if (! db.AktifSiparisler.Any(x => x.MasaNo == i))
                {
                    cboMasalar.Items.Add(i);
                }
            }
        }

        private void BlSiparisDetaylar_ListChanged(object sender, ListChangedEventArgs e)
        {
            OdemeTutariGuncelle();
        }

        private void OdemeTutariGuncelle()
        {
            lblOdemeTutari.Text = siparis.ToplamTutarTL;
        }

        private void UrunleriListele()
        {
            cboUrun.DataSource = db.Urunler;
        }

        private void MasaNoGuncelle()
        {
            Text = $"Masa {siparis.MasaNo:00} - Sipariş Detayları" +
                $"({siparis.AcilisZamani.Value.ToShortTimeString()})";
            lblMasaNo.Text = siparis.MasaNo.ToString("00");

        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            Urun secilenUrun = (Urun)cboUrun.SelectedItem;
            int adet = (int)nudAdet.Value;

            // SiparisDetay detay = blSiparisDetaylar.FirstOrDefault(x => x.UrunAd == secilenUrun.UrunAd);

            //if (detay != null )
            //{
            //    detay.Adet += adet;
            //    blSiparisDetaylar.ResetBindings();
            //}
            //else
            //{
            //    detay = new SiparisDetay()
            //    {
            //        UrunAd = secilenUrun.UrunAd,
            //        BirimFiyat = secilenUrun.BirimFiyat,
            //        Adet = adet
            //    }
            //     blSiparisDetaylar.Add(detay);
            //}

            SiparisDetay detay = new SiparisDetay()
            {
                UrunAd = secilenUrun.UrunAd,
                BirimFiyat = secilenUrun.BirimFiyat,
                Adet = adet

            };
            blSiparisDetaylar.Add(detay);

            //dgvSiparisDetaylar.DataSource = null;
            //dgvSiparisDetaylar.DataSource = siparis.SiparisDetaylar; binding list zaten data source a haber veriyor o yüzden bu 2 satıra gerek kalmadı.
        }



        private void dgvSiparisDetaylar_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {

            DialogResult dr = MessageBox.Show("Seçili detayları silmek istediğinize emin misiniz ?",
                "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (dr != DialogResult.Yes)
            {
                e.Cancel = true;
            }

        }

        private void btnAnaSayfa_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSiparisIptal_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Sipariş iptal edilerek kapatılacaktır. Emin misiniz ?",
               "İptal Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
               MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Yes)
            {
                SiparisKapat(SiparisDurumu.Iptal);
            }

        }

        private void btnOdemeAl_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Ödeme alındıysa sipariş kapatılacaktır. Emin misiniz ?",
                "Ödeme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Yes)
            {
                SiparisKapat(SiparisDurumu.Odendi, siparis.ToplamTutar());
            }



        }

        private void SiparisKapat(SiparisDurumu siparisDurumu, decimal odenenTutar = 0)
        {
            siparis.OdenenTutar = odenenTutar;
            siparis.KapanisZamani = DateTime.Now;
            siparis.Durum = siparisDurumu;
            db.AktifSiparisler.Remove(siparis);
            db.GecmisSiparisler.Add(siparis);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnMasaTasi_Click(object sender, EventArgs e)
        {
            if (cboMasalar.SelectedIndex < 0) return;
            int kaynak = siparis.MasaNo;
            int hedef = (int)cboMasalar.SelectedItem;
            siparis.MasaNo = hedef;
            anaForm.MasaTasi(kaynak, hedef);
            MasaNoGuncelle();
            MasalariListele();
        }
    }
}
