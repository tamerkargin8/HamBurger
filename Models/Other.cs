namespace HamBurger.Models
{
    public static class Other // Sipariş, Session ve diğer tüm işlemleri buradan halledeceğiz 
    {
        public const string Role_Person = "Person"; // Facebook ve Google girişleri için role verme
        public const string Role_Admin = "Admin"; // Admin Rolü için giriş
        public const string Role_User = "User"; // Kullanıcı Rolü

        public const string SShoppingCart = "Shopping Cart Session";
        public const string Siparis_Bekleyen = "Siparis_Bekleyen";
        public const string Siparis_Onaylandi = "Siparis_Onaylandi";
        public const string Siparis_Yolda = "Siparis_Yolda";
    }
}
