public class lista2 {
    public static void main(String[] args){

        int zakres = 100;                               //ustawiam zakres liczb

        int[] numall = new int[zakres-1];               //pusta tablica na liczby od 2 do n
        int[] primes = new int[zakres-1];               //pusta tablica na liczby pierwsze
        int nprimes = 0;

        int temp=2;                                     //zmienna pomocnicza - iterator

        while (temp <= zakres){                         //wypelniam pusta tablice liczbami od 2 do n
            numall[temp-2]=temp;
            temp++;
        }


        temp=0;                                        //iterator
        int wielokrotnosc;
        int dzielnik;

        while (temp<zakres/2){                         //dla kazdej liczby z numall zakres od 0 do zakres/2
            dzielnik=numall[temp];                     //dzielnik to ta liczba

            if (dzielnik!=0) {                         //jak dzielnik jest juz wczesniej wyzerowany to nic nie rob
                wielokrotnosc = dzielnik+dzielnik;     //zaczynamy od dwukrotnosci dzielnika

                while (wielokrotnosc <= zakres) {      //dopoki wielokrotnosc jest mniejsza od calego zakresu
                    numall[wielokrotnosc - 2] = 0;     //wyzeruj liczbe w tablicy liczb
                    wielokrotnosc += dzielnik;
                }
            }
            temp++;
        }


        temp=0;                                         //iterator po tablicy numall
        int num;                                        //obecna liczba z numall
        int temp2=0;                                    //miejsce do wstawienia w primes

        while(temp<zakres-1){                           //przejdz po tablicy numall
            num=numall[temp];
            if (num!=0){                                //jezeli liczba w numall jest rozna od 0
                primes[temp2]=num;                      //to przepisz ja do tablicy primes
                temp2++;
                nprimes = nprimes + 1;                  //i zwieksz licznik pierwszych
            }
            temp++;
        }

        temp=0;


        while (primes[temp]!=0){
            System.out.println(primes[temp]);           //wypisz primes
            temp++;
        }

    }
}
