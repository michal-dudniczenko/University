public class Circle extends Shape{
    private int radius;

    public Circle(Point position, boolean isFilled, int radius) {
        this.position = position;
        this.isFilled = isFilled;
        this.radius = radius;
    }

    public int getRadius(){
        return this.radius;
    }

    @Override
    public boolean getFilled() {
        return this.isFilled;
    }

    @Override
    public Point getPosition() {
        return this.position;
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
        maxDown = position.getY()-(radius*2);
        return new Point[] {
                new Point(maxLeft, maxUp),
                new Point(maxRight, maxUp),
                new Point(maxRight, maxDown),
                new Point(maxLeft, maxDown)
        };
    }

    @Override
    public void draw() {

    }
}
