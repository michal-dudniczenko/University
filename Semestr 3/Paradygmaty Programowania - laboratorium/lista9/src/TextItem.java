public class TextItem extends Item{
    private String text;

    public TextItem(Point position, String text) {
        this.position = position;
        this.text = text;
    }

    public String getText(){
        return this.text;
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
        String[] lines = text.split("\n");
        for (String s : lines){
            System.out.println(s);
        }
        int maxLength = lines[0].length();
        for (String s : lines){
            if (s.length()>maxLength){
                maxLength = s.length();
            }
        }

        int maxRight, maxLeft, maxUp, maxDown;
        maxRight = position.getX()+maxLength;
        maxLeft = position.getX();
        maxUp = position.getY();
        maxDown = position.getY()-lines.length;
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
