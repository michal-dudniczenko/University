import java.awt.*;

public class Circle extends Shape{
    private final int radius;

    public Circle(Point position, boolean isFilled, int radius) {
        this.position = position;
        this.isFilled = isFilled;
        this.radius = radius;
    }

    public int getRadius(){
        return this.radius;
    }

    @Override
    public void translate(Point p) {
        this.position = new Point(this.position.getX()+p.getX(), this.position.getY()+p.getY());
    }

    @Override
    public Point[] getBoundingBox() {
        int maxRight, maxLeft, maxUp, maxDown;
        maxRight = position.getX()+(radius*2);
        maxLeft = position.getX();
        maxUp = position.getY();
        maxDown = position.getY()+(radius*2);
        return new Point[] {
                new Point(maxLeft, maxUp),
                new Point(maxRight, maxUp),
                new Point(maxRight, maxDown),
                new Point(maxLeft, maxDown)
        };
    }

    @Override
    public void draw(Graphics g) {
        g.setColor(Color.BLACK);
        if (getFilled()) {
            g.fillOval(position.getX(), position.getY(), radius * 2, radius * 2);
        } else {
            g.drawOval(position.getX(), position.getY(), radius * 2, radius * 2);
        }

        if (isBoundVisible){
            drawBoundingBox(g);
        }
    }
}