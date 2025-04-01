import java.util.ArrayList;

public interface Singleton {
    default void removeOccurrences(ArrayList<Item> items) {
        items.removeIf(obj -> obj.getClass().equals(this.getClass()));
    }
}