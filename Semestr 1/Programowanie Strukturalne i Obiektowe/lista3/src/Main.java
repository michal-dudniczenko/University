public class Main {
    public static void main(String[] args) {
        Tablica1D t1 = new Tablica1D(5);
        t1.wypelnij();
        t1.wyswietl();
        t1.wyswietl_od_tylu();
        System.out.println(t1.maksimum());
        System.out.println(t1.minimum());
        t1.wyswietl(t1.parzyste());
        t1.wyswietl(t1.nieparzyste());

        Macierz m1 = new Macierz(5);
        Macierz m2 = new Macierz(5);
        m1.wypisz();
        m2.wypisz();
        m1.wypisz(m1.suma(m2));
        m1.wypisz(m1.iloczyn(m2));
    }
}