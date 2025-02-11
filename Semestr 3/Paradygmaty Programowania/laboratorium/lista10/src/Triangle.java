import java.awt.*;

public class Triangle extends Shape implements Singleton{
    private Point p1;
    private Point p2;
    private Point p3;

    public Triangle(boolean isFilled, Point p1, Point p2, Point p3) {
        int maxLeft, maxUp;
        maxLeft = Math.min(p1.getX(), Math.min(p2.getX(), p3.getX()));
        maxUp = Math.max(p1.getY(), Math.max(p2.getY(), p3.getY()));

        this.position = new Point(maxLeft, maxUp);
        this.isFilled = isFilled;
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
    }

    public Point getP1() {
        return p1;
    }

    public Point getP2() {
        return p2;
    }

    public Point getP3() {
        return p3;
    }

    @Override
    public void translate(Point p) {
        this.position = new Point(this.position.getX()+p.getX(), this.position.getY()+p.getY());
        this.p1 = new Point(this.p1.getX()+p.getX(), this.p1.getY()+p.getY());
        this.p2 = new Point(this.p2.getX()+p.getX(), this.p2.getY()+p.getY());
        this.p3 = new Point(this.p3.getX()+p.getX(), this.p3.getY()+p.getY());
    }

    @Override
    public Point[] getBoundingBox() {
        int maxRight, maxLeft, maxUp, maxDown;
        maxRight = Math.max(p1.getX(), Math.max(p2.getX(), p3.getX()));
        maxLeft = position.getX();
        maxUp = position.getY();
        maxDown = Math.min(p1.getY(), Math.min(p2.getY(), p3.getY()));
        return new Point[] {
                new Point(maxLeft, maxUp),
                new Point(maxRight, maxUp),
                new Point(maxRight, maxDown),
                new Point(maxLeft, maxDown)
        };
    }

    @Override
    public void draw(Graphics g) {
        int[] xPoints = {p1.getX(), p2.getX(), p3.getX()};
        int[] yPoints = {p1.getY(), p2.getY(), p3.getY()};

        g.setColor(Color.BLACK);
        if (getFilled()) {
            g.fillPolygon(xPoints, yPoints, 3);
        } else {
            g.drawPolygon(xPoints, yPoints, 3);
        }

        if (isBoundVisible){
            drawBoundingBox(g);
        }
    }
}