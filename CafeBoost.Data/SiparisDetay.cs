using System;
using System.Collections.Generic;
using System.Text;

namespace CafeBoost.Data
{
    public class SiparisDetay
    {
        public string UrunAd { get; set; }
        public decimal BirimFiyat { get; set; }
        public int Adet { get; set; }
        public string TutarTL { get { return $"{Tutar():0.00}TL"; } } //Datagrideview de sütun oluşturabiliriz.

        public decimal Tutar() => Adet * BirimFiyat;

        //public decimal Tutar()
        //{
        //    return Adet * BirimFiyat;
        //}





    }

   
}
