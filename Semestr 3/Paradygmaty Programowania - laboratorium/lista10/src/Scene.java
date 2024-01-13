import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;
import java.util.ArrayList;
import javax.swing.*;
import java.awt.*;

public class Scene extends JFrame {

    private class DrawingPanel extends JPanel{
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

    private class BoundingBoxDecorator extends DrawingPanel implements MouseListener{
        private Item highlightedItem = null;
        private final DrawingPanel wrappee;

        public BoundingBoxDecorator(DrawingPanel wrappee) {
            this.wrappee = wrappee;
            addMouseListener(this);
            setOpaque(false);
        }

        @Override
        protected void paintComponent(Graphics g) {
            wrappee.paintComponent(g);
            if (highlightedItem != null && highlightedItem.isBoundVisible) {
                highlightedItem.drawBoundingBox(g);
            }
        }

        @Override
        public void mouseClicked(MouseEvent e) {
            java.awt.Point clickPoint = e.getPoint();
            for (Item item : items) {
                if (isPointInsideBoundingBox(clickPoint, item.getBoundingBox())) {
                    highlightedItem = item;
                    item.setBoundVisible(true);
                } else {
                    item.setBoundVisible(false);
                }
            }
            repaint();
        }

        @Override
        public void mousePressed(MouseEvent e) {
        }

        @Override
        public void mouseReleased(MouseEvent e) {
        }

        @Override
        public void mouseEntered(MouseEvent e) {
        }

        @Override
        public void mouseExited(MouseEvent e) {
            highlightedItem = null;
            for (Item item : items) {
                item.setBoundVisible(false);
            }
            repaint();
        }

        private boolean isPointInsideBoundingBox(java.awt.Point clickPoint, Point[] boundingBox) {
            Polygon polygon = new Polygon();
            for (Point point : boundingBox) {
                polygon.addPoint(point.getX(), point.getY());
            }
            return polygon.contains(clickPoint);
        }
    }

    private ArrayList<Item> items;

    public Scene() {
        this.items = new ArrayList<>();
        add(new BoundingBoxDecorator(new DrawingPanel()));
    }

    public void addItem(Item item) {
        if (item instanceof Singleton){
            ((Singleton) item).removeOccurrences(this.items);
        }
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