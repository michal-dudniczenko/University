package Frontend;

import Backend.*;
import javax.swing.*;
import javax.swing.border.EmptyBorder;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

public class PrzyciskDodajPracownikaBD{
    public JFrame frame;

    public PrzyciskDodajPracownikaBD() {
        SwingUtilities.invokeLater(new Runnable() {
            @Override
            public void run() {
                frame = new JFrame("Dodawanie pracownika badawczo-dydaktycznego");
                frame.add(new Panel1());
                frame.pack();
                frame.setLocationRelativeTo(null);
                frame.setDefaultCloseOperation(JFrame.DO_NOTHING_ON_CLOSE);
                frame.setResizable(false);
                frame.setVisible(true);
            }
        });
    }

    public class Panel1 extends JPanel {
        String imie = "";
        String nazwisko = "";
        String PESEL = "";
        int wiek = 0;
        String plec = "m";
        String stanowisko = "Asystent";
        int stazPracy = 0;
        int pensja = 0;
        int liczbaPublikacji = 0;

        public Panel1() {
            setBorder(new EmptyBorder(10, 10, 10, 10));
            setLayout(new GridBagLayout());

            GridBagConstraints gbc = new GridBagConstraints();
            gbc.gridwidth = GridBagConstraints.REMAINDER;
            gbc.anchor = GridBagConstraints.NORTH;
            JLabel naglowek = new JLabel("Podaj dane pracownika:");
            naglowek.setFont(new Font("a", Font.BOLD, 15));
            add(naglowek, gbc);
            add(new JLabel(" "), gbc);

            gbc.anchor = GridBagConstraints.CENTER;
            gbc.fill = GridBagConstraints.HORIZONTAL;

            JPanel elementy = new JPanel(new GridBagLayout());

            JLabel label1 = new JLabel("Podaj imię:");
            elementy.add(label1, gbc);
            JTextField textField1 = new JTextField();
            elementy.add(textField1, gbc);

            JLabel label2 = new JLabel("Podaj nazwisko:");
            elementy.add(label2, gbc);
            JTextField textField2 = new JTextField();
            elementy.add(textField2, gbc);

            JLabel label3 = new JLabel("Podaj PESEL:");
            elementy.add(label3, gbc);
            JTextField textField3 = new JTextField();
            elementy.add(textField3, gbc);

            JLabel label4 = new JLabel("Podaj wiek:");
            elementy.add(label4, gbc);
            JTextField textField4 = new JTextField();
            elementy.add(textField4, gbc);

            JLabel label7 = new JLabel("Podaj staż pracy:");
            elementy.add(label7, gbc);
            JTextField textField7 = new JTextField();
            elementy.add(textField7, gbc);

            JLabel label8 = new JLabel("Podaj pensję:");
            elementy.add(label8, gbc);
            JTextField textField8 = new JTextField();
            elementy.add(textField8, gbc);

            JLabel label9 = new JLabel("Podaj liczbę publikacji:");
            elementy.add(label9, gbc);
            JTextField textField9 = new JTextField();
            elementy.add(textField9, gbc);

            JLabel label5 = new JLabel("Wybierz płeć:");
            elementy.add(label5, gbc);
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
            //Asystent, Adiunkt, Profesor Nadzwyczajny, Profesor Zwyczajny, Wykładowca
            JLabel label6 = new JLabel("Wybierz stanowisko:");
            elementy.add(label6, gbc);
            JPanel wyborStanowiska = new JPanel(new FlowLayout());
            JCheckBox checkBox3 = new JCheckBox("Asystent");wyborStanowiska.add(checkBox3);
            JCheckBox checkBox4 = new JCheckBox("Adiunkt");wyborStanowiska.add(checkBox4);
            JCheckBox checkBox5 = new JCheckBox("Prof. Nadzwyczajny");wyborStanowiska.add(checkBox5);
            JCheckBox checkBox6 = new JCheckBox("Prof. Zwyczajny");wyborStanowiska.add(checkBox6);
            JCheckBox checkBox7 = new JCheckBox("Wykładowca");wyborStanowiska.add(checkBox7);
            checkBox3.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    if (checkBox4.isSelected())checkBox4.setSelected(false);
                    if (checkBox5.isSelected())checkBox5.setSelected(false);
                    if (checkBox6.isSelected())checkBox6.setSelected(false);
                    if (checkBox7.isSelected())checkBox7.setSelected(false);
                }
            });
            checkBox4.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    if (checkBox3.isSelected())checkBox3.setSelected(false);
                    if (checkBox5.isSelected())checkBox5.setSelected(false);
                    if (checkBox6.isSelected())checkBox6.setSelected(false);
                    if (checkBox7.isSelected())checkBox7.setSelected(false);
                }
            });
            checkBox5.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    if (checkBox3.isSelected())checkBox3.setSelected(false);
                    if (checkBox4.isSelected())checkBox4.setSelected(false);
                    if (checkBox6.isSelected())checkBox6.setSelected(false);
                    if (checkBox7.isSelected())checkBox7.setSelected(false);
                }
            });
            checkBox6.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    if (checkBox3.isSelected())checkBox3.setSelected(false);
                    if (checkBox4.isSelected())checkBox4.setSelected(false);
                    if (checkBox5.isSelected())checkBox5.setSelected(false);
                    if (checkBox7.isSelected())checkBox7.setSelected(false);
                }
            });
            checkBox7.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    if (checkBox3.isSelected())checkBox3.setSelected(false);
                    if (checkBox4.isSelected())checkBox4.setSelected(false);
                    if (checkBox5.isSelected())checkBox5.setSelected(false);
                    if (checkBox6.isSelected())checkBox6.setSelected(false);
                }
            });
            elementy.add(wyborStanowiska, gbc);

            elementy.add(new JLabel(" "), gbc);
            JButton button0 = new JButton("Zatwierdź");
            button0.addActionListener(new ActionListener() {
                public void actionPerformed(ActionEvent e) {
                    imie = textField1.getText();
                    nazwisko = textField2.getText();
                    PESEL = textField3.getText();
                    wiek = Integer.parseInt(textField4.getText());
                    if (checkBox2.isSelected())plec="k";
                    if (checkBox3.isSelected())stanowisko="Asystent";
                    else if (checkBox4.isSelected())stanowisko="Adiunkt";
                    else if (checkBox5.isSelected())stanowisko="Profesor Nadzwyczajny";
                    else if (checkBox6.isSelected())stanowisko="Profesor Zwyczajny";
                    else if (checkBox7.isSelected())stanowisko="Wykładowca";

                    stazPracy = Integer.parseInt(textField7.getText());
                    pensja = Integer.parseInt(textField8.getText());
                    liczbaPublikacji = Integer.parseInt(textField9.getText());

                    Main.setOsoby(Funkcje.dodajOsobe(Main.getOsoby(), new PracownikBadawczoDydaktyczny(imie, nazwisko, PESEL, wiek, plec, stanowisko, stazPracy, pensja, liczbaPublikacji)));
                    new PrzyciskKontynuuj("Pomyślnie dodano pracownika.");
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
