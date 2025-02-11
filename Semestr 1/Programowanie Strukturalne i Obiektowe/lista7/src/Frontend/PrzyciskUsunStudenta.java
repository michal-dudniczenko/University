package Frontend;

import Backend.*;
import javax.swing.*;
import javax.swing.border.EmptyBorder;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

public class PrzyciskUsunStudenta{
    public JFrame frame;
    public PrzyciskUsunStudenta() {
        SwingUtilities.invokeLater(new Runnable() {
            @Override
            public void run() {
                frame = new JFrame("Usuwanie studentów");
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

            JLabel label1 = new JLabel("Wybierz filtr usuwania:");elementy.add(label1, gbc);
            JPanel wyborFiltru = new JPanel(new FlowLayout());
            JCheckBox checkBox1 = new JCheckBox("Nazwisko");wyborFiltru.add(checkBox1);
            JCheckBox checkBox2 = new JCheckBox("Imię");wyborFiltru.add(checkBox2);
            JCheckBox checkBox3 = new JCheckBox("Rok studiów");wyborFiltru.add(checkBox3);
            JCheckBox checkBox4 = new JCheckBox("Numer indeksu");wyborFiltru.add(checkBox4);
            checkBox1.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    if (checkBox2.isSelected())checkBox2.setSelected(false);
                    if (checkBox3.isSelected())checkBox3.setSelected(false);
                    if (checkBox4.isSelected())checkBox4.setSelected(false);
                }
            });
            checkBox2.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    if (checkBox1.isSelected())checkBox1.setSelected(false);
                    if (checkBox3.isSelected())checkBox3.setSelected(false);
                    if (checkBox4.isSelected())checkBox4.setSelected(false);
                }
            });
            checkBox3.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    if (checkBox2.isSelected())checkBox2.setSelected(false);
                    if (checkBox1.isSelected())checkBox1.setSelected(false);
                    if (checkBox4.isSelected())checkBox4.setSelected(false);
                }
            });
            checkBox4.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    if (checkBox2.isSelected())checkBox2.setSelected(false);
                    if (checkBox3.isSelected())checkBox3.setSelected(false);
                    if (checkBox1.isSelected())checkBox1.setSelected(false);
                }
            });
            elementy.add(wyborFiltru, gbc);

            JLabel label3 = new JLabel("Podaj frazę do usuwania:");elementy.add(label3, gbc);
            JTextField textField2 = new JTextField();elementy.add(textField2, gbc);

            elementy.add(new JLabel(" "), gbc);
            JButton button0 = new JButton("Zatwierdź");
            button0.addActionListener(new ActionListener(){
                public void actionPerformed(ActionEvent e){
                    String filtr = "1";
                    if (checkBox2.isSelected())filtr="2";
                    else if (checkBox3.isSelected())filtr="3";
                    else if (checkBox4.isSelected())filtr="4";

                    String fraza = textField2.getText();

                    Main.setOsoby(Funkcje.usunStudentow(Main.getOsoby(), filtr, fraza));
                    new PrzyciskKontynuuj("Usunięto studentów.");
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
