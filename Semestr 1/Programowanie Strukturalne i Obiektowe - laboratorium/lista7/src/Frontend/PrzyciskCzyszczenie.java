package Frontend;

import Backend.*;
import javax.swing.*;
import javax.swing.border.EmptyBorder;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

public class PrzyciskCzyszczenie {
    public JFrame frame;
    public PrzyciskCzyszczenie() {
        SwingUtilities.invokeLater(new Runnable() {
            @Override
            public void run() {
                frame = new JFrame("Czyszczenie listy studentów");
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
        public Panel1(){
            setBorder(new EmptyBorder(10, 10, 10, 10));
            setLayout(new GridBagLayout());

            GridBagConstraints gbc = new GridBagConstraints();
            gbc.gridwidth = GridBagConstraints.REMAINDER;
            gbc.anchor = GridBagConstraints.CENTER;
            gbc.fill = GridBagConstraints.HORIZONTAL;

            JPanel elementy = new JPanel(new GridBagLayout());

            JLabel label1 = new JLabel("Wybierz rodzaj czyszczenia listy:");elementy.add(label1, gbc);

            JPanel wyborCzyszczenia = new JPanel(new FlowLayout());
            JCheckBox checkBox1 = new JCheckBox("Usuń studentów z niewłaściwymi danymi");wyborCzyszczenia.add(checkBox1);
            JCheckBox checkBox2 = new JCheckBox("Usuń studentów niezapisanych na żadne kursy");wyborCzyszczenia.add(checkBox2);
            checkBox1.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    if (checkBox2.isSelected())checkBox2.setSelected(false);
                }
            });
            checkBox2.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    if (checkBox1.isSelected())checkBox1.setSelected(false);
                }
            });
            elementy.add(wyborCzyszczenia, gbc);

            elementy.add(new JLabel(" "), gbc);
            JButton button0 = new JButton("Zatwierdź");
            button0.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    String wybor = "1";
                    if (checkBox2.isSelected())wybor="2";
                    Main.setOsoby(Funkcje.czyszczenie(Main.getOsoby(), wybor));
                    new PrzyciskKontynuuj("Pomyślnie wyczyszczono listę studentów.");
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
