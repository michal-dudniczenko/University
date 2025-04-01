import java.util.Random;

public class PriorityQueue {
    private class Element{
        private int priority;
        private String value;

        public Element(String value, int priority){
            this.value=value;
            this.priority=priority;
        }

        public int getPriority() {
            return priority;
        }
        public void setPriority(int priority) {
            this.priority = priority;
        }
        public String getValue() {
            return value;
        }
        public void setValue(String value) {
            this.value = value;
        }

        public String toString(){
            return this.value+"-"+this.priority;
        }
    }

    private Element[] arr;
    private int elements;

    public PriorityQueue(int size){
        this.arr =new Element[size];
        this.elements=0;
    }

    private static void swap(Element[] arr, int n1, int n2){
        Element swap=arr[n1];
        arr[n1]=arr[n2];
        arr[n2]=swap;
    }

    private static void heapify(Element[] arr, int heapSize, int node){
        int largest = node;
        int left = 2 * node + 1;
        int right = 2 * node + 2;

        if (left<heapSize && arr[left].getPriority()>arr[largest].getPriority()){
            largest=left;
        }
        if (right<heapSize && arr[right].getPriority()>arr[largest].getPriority()){
            largest=right;
        }

        if (largest!=node){
            swap(arr, largest, node);
            heapify(arr, heapSize, largest);
        }
    }

    public String enqueue(String value, int priority){
        if (this.elements==this.arr.length){
            Element[] temp = new Element[this.elements+1];
            int i=0;
            for (Element e : this.arr){
                temp[i]=e;
                i++;
            }
            this.arr =temp;
        }

        Element temp = new Element(value,priority);

        this.arr[this.elements]=temp;
        this.elements++;

        for (int i=this.elements/2-1; i>=0; i--){
            heapify(arr, this.elements, i);
        }
        return temp.getValue();
    }

    public String dequeue(){
        String temp=this.arr[0].getValue();
        swap(this.arr, 0, this.elements-1);
        this.arr[this.elements-1]=null;
        this.elements--;
        heapify(this.arr, this.elements, 0);
        return temp;
    }

    public void changePriority(int index, int priority){
        if (index>this.elements-1)throw new IndexOutOfBoundsException();
        this.arr[index].setPriority(priority);

        for (int i=elements/2-1; i>=0; i--){
            heapify(arr, elements, i);
        }
    }

    public String remove(int index){
        if (index>this.elements-1)throw new IndexOutOfBoundsException();
        this.elements--;
        String temp=this.arr[index].getValue();
        for (int i=index; i<this.elements; i++){
            this.arr[i]=this.arr[i+1];
        }
        this.arr[this.elements]=null;

        for (int i=this.elements/2-1; i>=0; i--){
            heapify(arr, this.elements, i);
        }
        return temp;
    }

    public void display(){
        System.out.println("Zawartość kolejki:");
        int i=0;
        int tab=10;
        int power=0;
        while(i<this.elements){
            for (int t=0; t<tab; t++) System.out.print("\t");
            int temp=(int)Math.pow(2, power)+i;
            power++;
            while(i<temp && i<this.elements){
                System.out.print("("+this.arr[i]+") ");
                i++;
            }
            tab-=(i+Math.pow(2, i-1)-1);
            System.out.print("\n");
        }
        System.out.print("\n");
    }

    public static void main(String[] args){
        System.out.println("--------------------------------------");

        PriorityQueue priorityQueue = new PriorityQueue(10);

        Random generator = new Random();

        priorityQueue.enqueue("Zosia", generator.nextInt(1, 11));
        priorityQueue.enqueue("Zuzia", generator.nextInt(1, 11));
        priorityQueue.enqueue("Hania", generator.nextInt(1, 11));
        priorityQueue.enqueue("Maja", generator.nextInt(1, 11));
        priorityQueue.enqueue("Laura", generator.nextInt(1, 11));
        priorityQueue.enqueue("Antoni", generator.nextInt(1, 11));
        priorityQueue.enqueue("Jan", generator.nextInt(1, 11));
        priorityQueue.enqueue("Aleksander", generator.nextInt(1, 11));
        priorityQueue.enqueue("Franciszek", generator.nextInt(1, 11));
        priorityQueue.enqueue("Nikodem", generator.nextInt(1, 11));

        priorityQueue.display();

        System.out.println("\ndequeue: "+priorityQueue.dequeue()+"\n");
        priorityQueue.display();

        System.out.println("\ndequeue: "+priorityQueue.dequeue()+"\n");
        priorityQueue.display();

        System.out.println("\nenqueue: "+priorityQueue.enqueue("Michał", 9)+"\n");
        priorityQueue.display();

        System.out.println("\nremove: "+priorityQueue.remove(2)+"\n");
        priorityQueue.display();

        System.out.println("\nzmiana priorytetu: index=3, nowy priorytet=8\n");
        priorityQueue.changePriority(3, 8);

        priorityQueue.display();
    }
}
