using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CafeBoost.Data
{
    public class Siparis
    {
        public int MasaNo { get; set; }
        public List<SiparisDetay> SiparisDetaylar { get; set; }
        public DateTime? AcilisZamani { get; set; }
        public DateTime? KapanisZamani { get; set; }
        public SiparisDurumu Durum { get; set; }
        public decimal OdenenTutar { get; set; }
        public string ToplamTutarTL => $"{ToplamTutar():0.00}₺";

        //public decimal ToplamTutar()
        //{
        //    return SiparisDetaylar.Sum(x => x.Tutar());
        //}

        public Siparis() 
        {
            SiparisDetaylar = new List<SiparisDetay>();
            AcilisZamani = DateTime.Now;
        }
        public decimal ToplamTutar() => SiparisDetaylar.Sum(x => x.Tutar());


        //public decimal ToplamTutarTL
        //{
        //    decimal toplam = 0;
        //    foreach (var item in SiparisDetaylar)
        //  {
        //    toplam += item.Adet* item.Birimfiyat;
        // }

        //    Return toplam;
        //}

    }
}
