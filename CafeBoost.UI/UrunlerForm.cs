using CafeBoost.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CafeBoost.UI
{
    public partial class UrunlerForm : Form
    {
        private readonly KafeVeri db;
        BindingList<Urun> blUrunler;
        public UrunlerForm(KafeVeri kafeVeri)
        {
            db = kafeVeri;
            blUrunler = new BindingList<Urun>(db.Urunler);
            InitializeComponent();
            dgvUrunler.DataSource = blUrunler;
        }



        private void btnEkle_Click(object sender, EventArgs e)
        {
            string urunAd = txtUrunAt.Text.Trim();

            if (urunAd == string.Empty)
            {
                errorProvider1.SetError(txtUrunAt, "Ürün adı girmediniz.");
                return;
            }
            if (UrunVarMi(urunAd))
            {
                errorProvider1.SetError(txtUrunAt, "Ürün zaten tanımlı.");
                return;

            }

            blUrunler.Add(new Urun()
            {
                UrunAd = urunAd,
                BirimFiyat = nudBirimFiyat.Value
            });

            txtUrunAt.Clear();
            nudBirimFiyat.Value = 0;


        }

        private void txtUrunAt_Validating(object sender, CancelEventArgs e)
        {
            if (txtUrunAt.Text.Trim() == "")
            {
                errorProvider1.SetError(txtUrunAt, "Ürün adı girmediniz.");
            }
            else
            {
                errorProvider1.SetError(txtUrunAt, "");
            }
        }

        private void dgvUrunler_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {

            Urun urun = (Urun)dgvUrunler.Rows[e.RowIndex].DataBoundItem;
           string mevcutDeger = dgvUrunler.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();

            //mevcut hücrede değişiklik yapılmadıysa veya yapıldı ama değer aynı kaldıysa

            if (!dgvUrunler.IsCurrentCellDirty || e.FormattedValue.ToString() == mevcutDeger)
            {
                return;
            }


            if (e.ColumnIndex == 0)
            {
                if (e.FormattedValue.ToString() == "")
                {
                    MessageBox.Show("Ürün adı boş girilemez.");
                    e.Cancel = true;
                }

                if (BaskaUrunVarmi(e.FormattedValue.ToString(),urun))
                {
                    MessageBox.Show("Ürün zaten mevcut");
                    e.Cancel = true;
                }
            }

            else if (e.ColumnIndex == 1)
            {
                decimal birimFiyat;
                bool gecerliMi = decimal.TryParse(e.FormattedValue.ToString(), out birimFiyat);

                if (!gecerliMi || birimFiyat < 0)
                {
                    MessageBox.Show("Geçersiz fiyat.");
                    e.Cancel = true;
                }

            }
        }

        private bool UrunVarMi(string UrunAd)
        {
            return db.Urunler.Any(x => x.UrunAd.Equals( UrunAd, StringComparison.CurrentCultureIgnoreCase));
        }

        private bool BaskaUrunVarmi(string urunAd, Urun urun)
        {
            return db.Urunler.Any(
                x => x.UrunAd.Equals(urunAd, StringComparison.CurrentCultureIgnoreCase) && x != urun);
        }


    }
}
