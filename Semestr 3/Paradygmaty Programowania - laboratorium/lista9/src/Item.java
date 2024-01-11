public abstract class Item {
    protected Point position;

    public abstract Point getPosition();
    public abstract void translate(Point p);
    public abstract Point[] getBoundingBox();
    public abstract void draw();
}