import java.awt.*;

public class Star extends Shape{
    private Point[] vertices;
    private final int size;

    public Star(Point position, boolean isFilled, int size) {
        this.position = position;
        this.isFilled = isFilled;
        this.size = size;

        double angleIncrement = 2 * Math.PI / 5;
        double startAngle = -Math.PI / 2;
        vertices = new Point[10];

        for (int i = 0; i < 5; i++) {
            double xOuter = position.getX() + size * Math.cos(startAngle + i * angleIncrement);
            double yOuter = position.getY() + size * Math.sin(startAngle + i * angleIncrement);
            vertices[i * 2] = new Point((int) xOuter, (int) yOuter);
        }

        for (int i = 0; i < 5; i++) {
            double xInner = position.getX() + size / 2 * Math.cos(startAngle + (i + 0.5) * angleIncrement);
            double yInner = position.getY() + size / 2 * Math.sin(startAngle + (i + 0.5) * angleIncrement);
            vertices[i * 2 + 1] = new Point((int) xInner, (int) yInner);
        }
    }

    public Point[] getVertices() {
        return vertices;
    }

    public int getSize() {
        return size;
    }

    @Override
    public void translate(Point p) {
        this.position = new Point(this.position.getX()+p.getX(), this.position.getY()+p.getY());
        for (int i=0; i< vertices.length; i++){
            vertices[i] = new Point(vertices[i].getX()+p.getX(), vertices[i].getY()+p.getY());
        }
    }

    @Override
    public Point[] getBoundingBox() {
        int maxRight, maxLeft, maxUp, maxDown;
        if (vertices.length != 0){
            maxRight = vertices[0].getX();
            maxLeft = vertices[0].getX();
            maxDown = vertices[0].getY();
            maxUp = vertices[0].getY();
        }else{
            maxRight = 0;
            maxLeft = Integer.MAX_VALUE;
            maxDown = 0;
            maxUp = Integer.MAX_VALUE;
        }
        for (int i=1; i<vertices.length; i++){
            if (vertices[i].getX() > maxRight){
                maxRight = vertices[i].getX();
            }
            if (vertices[i].getX() < maxLeft){
                maxLeft = vertices[i].getX();
            }
            if (vertices[i].getY() > maxDown){
                maxDown = vertices[i].getY();
            }
            if (vertices[i].getY() < maxUp){
                maxUp = vertices[i].getY();
            }
        }
        return new Point[] {
                new Point(maxLeft, maxUp),
                new Point(maxRight, maxUp),
                new Point(maxRight, maxDown),
                new Point(maxLeft, maxDown)
        };
    }

    @Override
    public void draw(Graphics g) {
        int n = vertices.length;
        int[] xPoints = new int[n];
        int[] yPoints = new int[n];

        for (int i=0; i<n; i++){
            xPoints[i] = vertices[i].getX();
            yPoints[i] = vertices[i].getY();
        }

        g.setColor(Color.YELLOW);
        if (getFilled()) {
            g.fillPolygon(xPoints, yPoints, n);
        } else {
            g.drawPolygon(xPoints, yPoints, n);
        }

        if (isBoundVisible){
            drawBoundingBox(g);
        }
    }
}
