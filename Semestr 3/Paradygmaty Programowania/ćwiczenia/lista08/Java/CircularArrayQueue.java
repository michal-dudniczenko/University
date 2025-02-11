import java.util.ArrayList;

public class CircularArrayQueue<E> implements MyQueue<E> {
    private int front;
    private int rear;
    private final int size;
    private final ArrayList<E> array;

    public CircularArrayQueue(int size) {
        this.front = 0;
        this.rear = 0;
        this.size = size;
        this.array = new ArrayList<>(size);
        for (int i = 0; i < size; i++) {
            this.array.add(null);
        }
    }

    @Override
    public void enqueue(E x) throws FullException {
        if (isFull()){
            throw new FullException("Array is full, cant enqueue");
        }
        else{
            this.array.set(this.rear % this.size, x);
            this.rear++;
        }
    }

    @Override
    public void dequeue() {
        if (!isEmpty()){
            this.front++;
        }
    }

    @Override
    public E first() throws EmptyException {
        if (isEmpty()) {
            throw new EmptyException("Array is empty, cant get first");
        }
        return this.array.get(this.front % this.size);
    }

    @Override
    public boolean isEmpty() {
        return this.front == this.rear;
    }

    @Override
    public boolean isFull() {
        return ((this.rear != this.front) && ((this.rear % this.size) == (this.front % this.size)));
    }

    public static void main(String[] args) {
        try {
            CircularArrayQueue<Integer> queue = new CircularArrayQueue<>(3);

            queue.enqueue(1);
            queue.enqueue(2);
            queue.enqueue(3);

            System.out.println("Queue is full: " + queue.isFull());

            System.out.println("Dequeue: " + queue.first());
            queue.dequeue();
            System.out.println("Dequeue: " + queue.first());
            queue.dequeue();
            System.out.println("Dequeue: " + queue.first());
            queue.dequeue();

            System.out.println("Queue is empty: " + queue.isEmpty());

        } catch (FullException | EmptyException e) {
            e.printStackTrace();
        }
    }
}
