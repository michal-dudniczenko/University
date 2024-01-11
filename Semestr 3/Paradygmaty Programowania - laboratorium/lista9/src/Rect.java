public class Rect extends Shape{
    private int width;
    private int height;

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
        maxRight = position.getX()+(width);
        maxLeft = position.getX();
        maxUp = position.getY();
        maxDown = position.getY()-(height);
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
