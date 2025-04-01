package Frontend;

import Backend.*;
import javax.swing.*;
import javax.swing.border.EmptyBorder;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.ArrayList;

public class PrzyciskDodajStudenta {
    public JFrame frame;
    public PrzyciskDodajStudenta() {
        SwingUtilities.invokeLater(new Runnable() {
            @Override
            public void run() {
                frame = new JFrame("Dodawanie studenta");
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
        String imie = "";
        String nazwisko = "";
        String PESEL = "";
        int wiek = 0;
        String plec = "m";
        String nrIndeksu = "";
        int rokStudiow = 0;
        ArrayList<Kursy> listaKursow = new ArrayList<Kursy>();
        boolean ERASMUS = false;
        boolean studia1stopnia = true;
        boolean stacjonarne = true;

        public Panel1(){
            setBorder(new EmptyBorder(10, 10, 10, 10));
            setLayout(new GridBagLayout());

            GridBagConstraints gbc = new GridBagConstraints();
            gbc.gridwidth = GridBagConstraints.REMAINDER;
            gbc.anchor = GridBagConstraints.NORTH;
            JLabel naglowek = new JLabel("Podaj dane studenta:");
            naglowek.setFont(new Font("a", Font.BOLD, 18));
            add(naglowek, gbc);

            gbc.anchor = GridBagConstraints.CENTER;
            gbc.fill = GridBagConstraints.HORIZONTAL;

            JPanel elementy = new JPanel(new GridBagLayout());

            JLabel label1 = new JLabel("Podaj imię:");elementy.add(label1, gbc);
            JTextField textField1 = new JTextField();elementy.add(textField1, gbc);

            JLabel label2 = new JLabel("Podaj nazwisko:");elementy.add(label2, gbc);
            JTextField textField2 = new JTextField();elementy.add(textField2, gbc);

            JLabel label3 = new JLabel("Podaj PESEL:");elementy.add(label3, gbc);
            JTextField textField3 = new JTextField();elementy.add(textField3, gbc);

            JLabel label4 = new JLabel("Podaj wiek:");elementy.add(label4, gbc);
            JTextField textField4 = new JTextField();elementy.add(textField4, gbc);

            JLabel label6 = new JLabel("Podaj numer indeksu:");elementy.add(label6, gbc);
            JTextField textField6 = new JTextField();elementy.add(textField6, gbc);

            JLabel label7 = new JLabel("Podaj rok studiów:");elementy.add(label7, gbc);
            JTextField textField7 = new JTextField();elementy.add(textField7, gbc);
            elementy.add(new JLabel(" "),gbc);

            JLabel label5 = new JLabel("Wybierz płeć:");elementy.add(label5, gbc);
            JPanel wyborPlci = new JPanel(new FlowLayout());
            JCheckBox checkBox1 = new JCheckBox("Mężczyzna");wyborPlci.add(checkBox1);
            JCheckBox checkBox2 = new JCheckBox("Kobieta");wyborPlci.add(checkBox2);
            elementy.add(wyborPlci, gbc);
            checkBox1.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    if (checkBox2.isSelected()){
                        checkBox2.setSelected(false);
                    }
                }
            });
            checkBox2.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    if (checkBox1.isSelected()){
                        checkBox1.setSelected(false);
                    }
                }
            });
            elementy.add(new JLabel(" "),gbc);
            JCheckBox checkBox3 = new JCheckBox("Czy student bierze udział w ERASMUS?");elementy.add(checkBox3, gbc);

            JCheckBox checkBox4 = new JCheckBox("Czy studia 2 stopnia?");elementy.add(checkBox4, gbc);

            JCheckBox checkBox5 = new JCheckBox("Czy studia niestacjonarne?");elementy.add(checkBox5, gbc);
            elementy.add(new JLabel(" "), gbc);
            JButton button1 = new JButton("Wybierz kursy");elementy.add(button1, gbc);
            button1.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    PrzyciskKursyStudenta temp = new PrzyciskKursyStudenta();
                    listaKursow = temp.getKursyTemp();
                }
            });
            elementy.add(new JLabel(" "), gbc);

            JButton button0 = new JButton("Zatwierdź");
            button0.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    imie = textField1.getText();
                    nazwisko = textField2.getText();
                    PESEL = textField3.getText();
                    wiek = Integer.parseInt(textField4.getText());
                    if (checkBox2.isSelected())plec="k";
                    nrIndeksu = textField6.getText();
                    rokStudiow = Integer.parseInt(textField7.getText());
                    if (checkBox3.isSelected())ERASMUS=true;
                    if (checkBox4.isSelected())studia1stopnia=false;
                    if (checkBox5.isSelected())stacjonarne=false;

                    Main.setOsoby(Funkcje.dodajOsobe(Main.getOsoby(), new Student(imie, nazwisko, PESEL, wiek, plec, nrIndeksu, rokStudiow, listaKursow, ERASMUS, studia1stopnia, stacjonarne)));
                    new PrzyciskKontynuuj("Pomyślnie dodano studenta.");
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
