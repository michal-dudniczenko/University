import java.awt.*;

public class Segment extends Primitive{
    private final int length;
    private Point start;
    private Point end;

    public Segment(Point start, Point end) {
        int maxLeft, maxUp;
        maxLeft = Math.min(start.getX(), end.getX());
        maxUp = Math.max(start.getY(), end.getY());
        this.position = new Point(maxLeft, maxUp);
        this.length = (int) Math.sqrt(Math.pow((end.getX()-start.getX()), 2) + Math.pow((end.getY()-start.getY()), 2));
        this.start = start;
        this.end = end;
    }

    public int getLength() {
        return length;
    }

    public Point getStart() {
        return start;
    }

    public Point getEnd() {
        return end;
    }

    @Override
    public void translate(Point p) {
        this.position = new Point(this.position.getX()+p.getX(), this.position.getY()+p.getY());
        this.start = new Point(this.start.getX()+p.getX(), this.start.getY()+p.getY());
        this.end = new Point(this.end.getX()+p.getX(), this.end.getY()+p.getY());
    }

    @Override
    public Point[] getBoundingBox() {
        int maxRight, maxLeft, maxUp, maxDown;
        maxRight = Math.max(start.getX(), end.getX());
        maxLeft = position.getX();
        maxUp = position.getY();
        maxDown = Math.min(start.getY(), end.getY());
        return new Point[] {
                new Point(maxLeft, maxUp),
                new Point(maxRight, maxUp),
                new Point(maxRight, maxDown),
                new Point(maxLeft, maxDown)
        };
    }

    @Override
    public void draw(Graphics g) {
        g.setColor(Color.GREEN);
        g.drawLine(start.getX(), start.getY(), end.getX(), end.getY());

        if (isBoundVisible){
            drawBoundingBox(g);
        }
    }
}
