package Frontend;

import Backend.*;
import javax.swing.*;
import javax.swing.border.EmptyBorder;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

public class PrzyciskDodajKurs {
    public JFrame frame;
    public PrzyciskDodajKurs() {
        SwingUtilities.invokeLater(new Runnable() {
            @Override
            public void run() {
                frame = new JFrame("Dodawanie kursu");
                frame.add(new Panel1());
                frame.pack();
                frame.setLocationRelativeTo(null);
                frame.setDefaultCloseOperation(JFrame.DO_NOTHING_ON_CLOSE);
                frame.setResizable(false);
                frame.setVisible(true);
            }
        });
    }
    public class Panel1 extends JPanel{
        String nazwaKursu;
        String nazwiskoProwadzacego;
        String imieProwadzacego;
        int ECTS;

        public Panel1(){
            setBorder(new EmptyBorder(10, 10, 10, 10));
            setLayout(new GridBagLayout());

            GridBagConstraints gbc = new GridBagConstraints();
            gbc.gridwidth = GridBagConstraints.REMAINDER;
            gbc.anchor = GridBagConstraints.NORTH;
            JLabel naglowek = new JLabel("Podaj dane kursu:");
            naglowek.setFont(new Font("a", Font.BOLD, 15));
            add(naglowek, gbc);
            add(new JLabel(" "), gbc);

            gbc.anchor = GridBagConstraints.CENTER;
            gbc.fill = GridBagConstraints.HORIZONTAL;

            JPanel elementy = new JPanel(new GridBagLayout());

            JLabel label1 = new JLabel("Podaj nazwę kursu:");elementy.add(label1, gbc);
            JTextField textField1 = new JTextField();elementy.add(textField1, gbc);

            JLabel label2 = new JLabel("Podaj imię prowadzącego:");elementy.add(label2, gbc);
            JTextField textField2 = new JTextField();elementy.add(textField2, gbc);

            JLabel label3 = new JLabel("Podaj nazwisko prowadzącego:");elementy.add(label3, gbc);
            JTextField textField3 = new JTextField();elementy.add(textField3, gbc);

            JLabel label4 = new JLabel("Podaj liczbę ECTS:");elementy.add(label4, gbc);
            JTextField textField4 = new JTextField();elementy.add(textField4, gbc);

            elementy.add(new JLabel(" "), gbc);
            JButton button0 = new JButton("Zatwierdź");
            button0.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    nazwaKursu = textField1.getText();
                    nazwiskoProwadzacego = textField3.getText();
                    imieProwadzacego = textField2.getText();
                    ECTS = Integer.parseInt(textField4.getText());

                    Main.setKursy(Funkcje.dodajKurs(Main.getKursy(), new Kursy(nazwaKursu, nazwiskoProwadzacego, imieProwadzacego, ECTS)));
                    new PrzyciskKontynuuj("Pomyślnie dodano kurs.");
                    frame.dispose();
                }
            });
            elementy.add(button0, gbc);

            JButton button0_1 = new JButton("Wróć");
            button0_1.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    frame.dispose();
                }
            });
            elementy.add(button0_1, gbc);

            gbc.weighty = 1;
            add(elementy, gbc);

        }
    }

}
