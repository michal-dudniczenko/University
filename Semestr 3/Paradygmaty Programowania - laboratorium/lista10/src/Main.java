import java.util.ArrayList;
import java.util.Arrays;
import java.util.Random;

public class Main {
    public static void main(String[] args) {
        Scene scene = new Scene();

        Segment grass = new Segment(new Point(50, 400), new Point(950, 400));
        scene.addItem(grass);

        Rect house = new Rect(new Point(200, 250), false, 200, 150);
        scene.addItem(house);
        Triangle roof = new Triangle(false, new Point(170, 250), new Point(430, 250), new Point(300, 100));
        scene.addItem(roof);
        Rect window1 = new Rect(new Point(220, 270), false, 40, 40);
        scene.addItem(window1);
        Rect window2 = new Rect(new Point(340, 270), false, 40, 40);
        scene.addItem(window2);
        Rect door = new Rect(new Point(270, 320), false, 60, 80);
        scene.addItem(door);
        //roof.setBoundVisible(true);

        Circle bottom = new Circle(new Point(600, 300), false, 60);
        Circle middle = new Circle(new Point(610, 200), false, 50);
        Circle head = new Circle(new Point(620, 120), false, 40);
        Circle leftEye = new Circle(new Point(640, 135), true, 5);
        Circle rightEye = new Circle(new Point(670, 135), true, 5);
        Rect nose = new Rect(new Point(658, 148), true, 4, 15);
        Triangle mouth = new Triangle(true, new Point(640, 170), new Point(680, 170), new Point(660, 180));

        ComplexItem snowman = new ComplexItem(new ArrayList<>(Arrays.asList(bottom, middle, head, leftEye, rightEye, nose, mouth)));
        scene.addItem(snowman);
        //snowman.setBoundVisible(true);
        snowman.translate(new Point(20, -20));

        TextItem text = new TextItem(new Point(0, 0), "Merry Christmas");
        text.translate(new Point(480, 250));
        //text.setBoundVisible(true);
        scene.addItem(text);

        Star star1 = new Star(new Point(50, 50), true, 20);
        //star1.setBoundVisible(true);
        scene.addItem(star1);

        Random rand = new Random();
        for (int i=0; i<18; i++){
            scene.addItem(new Star(new Point(50 + 50 * (i+1), rand.nextInt(30, 60)), true, 20));
        }

        scene.addItem(new Triangle(false, new Point(100, 450), new Point(140, 450), new Point(120, 470)));

        scene.draw();
    }
}