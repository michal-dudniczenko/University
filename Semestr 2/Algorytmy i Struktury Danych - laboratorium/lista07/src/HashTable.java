public class HashTable {
    private String[][] arr;
    private final int adresowanie;

    public HashTable(int size, int rodzajAdresowania){
        this.arr=new String[size][2];
        this.adresowanie=rodzajAdresowania;
    }

    private static int hashCode(String s){
        int suma=0;
        for (int i=0; i<s.length();i++){
            suma+=(int)s.charAt(i)*(i+7);
        }
        suma*= ((int)s.charAt(0)%17);
        return suma;
    }

    private int adresowanie(int i, String key){
        switch (this.adresowanie){
            case 1:{        //probkowanie liniowe
                return i;
            }
            case 2:{        //probkowanie kwadratowe
                return (int)(Math.pow(-1,i-1)*Math.pow((i+1)/2,2));
            }
            case 3:{        //podwojne mieszanie
                return  i*(hashCode(key)+7);
            }
            default:{
                return 0;
            }
        }
    }

    public String get(String key){
        int pozycja = hashCode(key) % this.arr.length;
        int pozycjaStart=pozycja;
        int i=1;
        while(this.arr[pozycja][0]!=null && !this.arr[pozycja][0].equals(key)){
            pozycja = pozycjaStart+adresowanie(i, key);
            pozycja%=this.arr.length;
            if (pozycja<0)pozycja=this.arr.length+pozycja;
            i++;
        }
        return this.arr[pozycja][1];
    }

    public void put(String key, String value){
        if (this.size()==this.arr.length)this.resize();
        String[] temp = {key, value};
        int pozycja = hashCode(key) % this.arr.length;
        int pozycjaStart=pozycja;
        System.out.println("pozycja startowa:"+pozycja);
        int i=1;
        while(this.arr[pozycja][0]!=null){
            pozycja = pozycjaStart+adresowanie(i, key);
            pozycja%=this.arr.length;
            if (pozycja<0)pozycja=this.arr.length+pozycja;
            i++;
            System.out.println("nowa pozycja="+pozycja);
        }
        this.arr[pozycja]=temp;
        System.out.println("finalna pozycja: "+pozycja);
    }

    public boolean containsKey(String key){
        return this.get(key)!=null;
    }

    public int size(){
        int licznik=0;
        for (int i=0; i<this.arr.length; i++){
            if(this.arr[i][0]!=null)licznik++;
        }
        return licznik;
    }

    public boolean isEmpty(){
        return this.size()==0;
    }

    public void resize(){
        int newSize=this.size()*2;

        String[][] temp = this.arr;
        this.arr=new String[newSize][2];

        for (int i=0; i<temp.length; i++){
            if (temp[i][0]!=null){
                this.put(temp[i][0], temp[i][1]);
            }
        }
    }

    public void dump(){
        System.out.print("\n{ ");
        for (String[] s : this.arr){
            System.out.print("["+s[0]+","+s[1]+"]"+", ");
        }
        System.out.print("}\n");
    }
}
