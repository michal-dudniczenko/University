import java.util.ArrayList;
import javax.swing.*;
import java.awt.*;

public class Scene extends JFrame {
    private class DrawingPanel extends JPanel {
        @Override
        protected void paintComponent(Graphics g) {
            super.paintComponent(g);
            for (Item item : items) {
                item.draw(g);
            }
        }

        @Override
        public Dimension getPreferredSize() {
            return new Dimension(1000, 500);
        }
    }

    private ArrayList<Item> items;

    public Scene() {
        this.items = new ArrayList<>();
        DrawingPanel drawingPanel = new DrawingPanel();
        add(drawingPanel);
    }

    public void addItem(Item item) {
        items.add(item);
    }

    public void draw() {
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        pack();
        setLocationRelativeTo(null);
        setTitle("Scene");
        setVisible(true);
    }
}