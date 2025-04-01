public class TestHashTable {
    public static void main(String[] args){
        //wybor adresowania przy tworzeniu:
        //1-liniowe
        //2-kwadratowe
        //3-podwojne hashowanie
        HashTable hashTable = new HashTable(5, 1);

        System.out.println("------------------------------------");

        System.out.println("\nisEmpty: "+hashTable.isEmpty());

        System.out.println("\nput: 1234 Nadia");hashTable.put("1234", "Nadia");
        hashTable.dump();System.out.println("size: "+hashTable.size());

        System.out.println("\nput: 3516 Kasia");hashTable.put("3516", "Kasia");
        hashTable.dump();System.out.println("size: "+hashTable.size());

        System.out.println("\nput: 8761 Michał");hashTable.put("8761", "Michał");
        hashTable.dump();System.out.println("size: "+hashTable.size());

        System.out.println("\nput: 0988 Tomek");hashTable.put("0988", "Tomek");
        hashTable.dump();System.out.println("size: "+hashTable.size());

        System.out.println("\nput: 7777 Ania");hashTable.put("7787", "Ania");
        hashTable.dump();System.out.println("size: "+hashTable.size());

        System.out.println("\nget: 8761");
        System.out.println(hashTable.get("8761"));

        System.out.println("\nresize\n");
        hashTable.resize();
        hashTable.dump();System.out.println("size: "+hashTable.size());

        System.out.println("\nget: 7787");

        System.out.println(hashTable.get("7787"));

        System.out.println("\nget: 3333");
        System.out.println(hashTable.get("3333"));

        System.out.println("\ncontainsKey: 3333");
        System.out.println(hashTable.containsKey("3333"));

        System.out.println("\ncontainsKey: 1234");
        System.out.println(hashTable.containsKey("1234"));

        System.out.println("\nisEmpty: "+hashTable.isEmpty());
    }
}
