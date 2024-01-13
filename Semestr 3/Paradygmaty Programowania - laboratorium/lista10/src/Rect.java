import java.awt.*;

public class Rect extends Shape{
    private final int width;
    private final int height;

    public Rect(Point position, boolean isFilled, int width, int height) {
        this.position = position;
        this.isFilled = isFilled;
        this.width = width;
        this.height = height;
    }

    public int getWidth() {
        return width;
    }

    public int getHeight() {
        return height;
    }

    @Override
    public void translate(Point p) {
        this.position = new Point(this.position.getX()+p.getX(), this.position.getY()+p.getY());
    }

    @Override
    public Point[] getBoundingBox() {
        int maxRight, maxLeft, maxUp, maxDown;
        maxRight = position.getX()+(width);
        maxLeft = position.getX();
        maxUp = position.getY();
        maxDown = position.getY()+(height);
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
            g.fillRect(position.getX(), position.getY(), width, height);
        } else {
            g.drawRect(position.getX(), position.getY(), width, height);
        }

        if (isBoundVisible){
            drawBoundingBox(g);
        }
    }
}
