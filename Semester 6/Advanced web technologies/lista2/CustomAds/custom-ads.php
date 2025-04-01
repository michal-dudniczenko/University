<?php
/*
 * Plugin Name: Custom Ads
*/

// klasa reprezentujaca kafelek ogloszenia w menu admina
class AdTile {
    private $adHtml;
    private $isEnabled;

    public function __construct($adHtml) {
        $this->adHtml = $adHtml;
        $this->isEnabled = false;
    }

    public function switchEnabled() {
        $this->isEnabled = !$this->isEnabled;
    }

    public function isEnabled() {
        return $this->isEnabled;
    }

    public function getAdHtml() {
        return $this->adHtml;
    }

    public function setAdHtml($adHtml) {
        $this->adHtml = $adHtml;
    }
}

// dodanie opcji w menu admina
function customAdsAdminSettingsOption(){
    add_options_page(
        "Custom Ads config", 
        "Custom Ads", 
        "manage_options", 
        "custom-ads", 
        "customAdsAdminPage"
    );
}
add_action("admin_menu", "customAdsAdminSettingsOption");

// generuje strone ustawien ogloszen
function customAdsAdminPage() {
    global $_POST;

    // get existing array of ads or initialize it if not found
    $adTiles = get_option("adsArray");

    if (!$adTiles) {
        $adTiles = array();
        update_option("adsArray", $adTiles);
    }

    $message = '';

    if (isset($_POST["manageAds"])) {
        switch ($_POST["manageAds"]) {
            case "add":
                $newAdHtml = $_POST["newAdHtml"];
                if (empty(trim($newAdHtml))) {
                    $message = '
                    <div class="notice notice-error is-dismissable">
                        <p>Ad content cannot be empty.</p>
                    </div>
                ';
                } else {
                    array_push($adTiles, new AdTile($newAdHtml));
                    $message = '
                        <div class="notice notice-success is-dismissable">
                            <p>New ad added.</p>
                        </div>
                    ';
                }
                break;
            case "edit":
                $newAdHtml = $_POST["newAdHtml"];
                if (empty(trim($newAdHtml))) {
                    $message = '
                        <div class="notice notice-error is-dismissable">
                            <p>Ad content cannot be empty.</p>
                        </div>
                    ';
                } else {
                    $adIndex = $_POST["adIndex"];
                    $adTiles[$adIndex]->setAdHtml($newAdHtml);
                    $message = '
                        <div class="notice notice-success is-dismissable">
                            <p>Ad edited.</p>
                        </div>
                    ';
                }
                break;
            case "switch":
                $adIndex = $_POST["adIndex"];
                $adTiles[$adIndex]->switchEnabled();
                $whatWasDone = "disabled";
                if ($adTiles[$adIndex]->isEnabled()) {
                    $whatWasDone = "enabled";
                }
                $message = "
                    <div class=\"notice notice-success is-dismissable\">
                        <p>Ad $whatWasDone.</p>
                    </div>
                ";
                break;
            case "remove":
                $adIndex = $_POST["adIndex"];
                array_splice($adTiles, $adIndex,1);
                $message = '
                    <div class="notice notice-success is-dismissable">
                        <p>Ad removed.</p>
                    </div>
                ';
                break;
        }
    }

    if ($message) {
        update_option("adsArray", $adTiles);
        echo $message;
    }
?>
    <form class="ad-form new-ad-form" method="post">
        <div class="vertical-display">
            <label for="newAdHtml">Ad content</label>
            <?php 
            $content = "";
            $editor_id = 'newAdHtml'; // The ID used for the editor
            $settings = array(
                'textarea_name' => 'newAdHtml', // Name used in the form
                'media_buttons' => false, // Show media upload button
                'teeny' => false, // Use full TinyMCE editor
                'quicktags' => true, // Enable quicktags (HTML mode)
                'textarea_rows' => 4,
            );
            wp_editor($content, $editor_id, $settings);
            ?>
        </div>
        <button class="new-ad-button ad-form-button" type="submit" name="manageAds" value="add">Create new ad</button>  
    </form>

<?php

    $index = count($adTiles);
    while ($index > 0) {
        $index--;
        $tile = $adTiles[$index];
?>
        <form class="ad-form <?php echo $tile->isEnabled() ? "enabled-ad-form" : "disabled-ad-form" ?>" method="post" >
            <div class="vertical-display">
                <input type="hidden" name="adIndex" value="<?php echo $index?>">
                <label for="newAdHtml">Ad content</label>
                <?php 
                    $content = $tile->getAdHtml();
                    $editor_id = 'adEditor' . $index; // The ID used for the editor
                    $settings = array(
                        'textarea_name' => 'newAdHtml', // Name used in the form
                        'media_buttons' => false, // Show media upload button
                        'teeny' => false, // Use full TinyMCE editor
                        'quicktags' => true, // Enable quicktags (HTML mode)
                        'textarea_rows' => 4,
                    );
                    wp_editor($content, $editor_id, $settings);
                ?>
                <!-- <textarea class="new-ad-form-textarea" name="newAdHtml" rows="4" required><?php //echo $tile->getAdHtml() ?></textarea> -->
            </div>
            <div class="buttons-group">
                <button class="ad-form-button" type="submit" name="manageAds" value="edit">Edit ad</button>
                <button class="ad-form-button" type="submit" name="manageAds" value="remove">Remove ad</button>
                <button class="ad-form-button" type="submit" name="manageAds" value="switch"><?php echo $tile->isEnabled() ? "Disable ad" : "Enable ad" ?></button>  
            </div>
        </form>
<?php
    }
}

// losuje i wstawia ogloszenie pomiedzy tytul a content posta
function insertAdBeforeContent($content) {
    $adTiles = get_option("adsArray");

    $enabledAds = array_values(array_filter($adTiles, function ($ad) {
        return $ad->isEnabled();
    }));

    if (count($enabledAds) > 0) {
        $custom_html = '</a></h2><div class="custom-adx">' 
        . $enabledAds[rand(0, count($enabledAds) -1)]->getAdHtml()
        . '</div><h2 style="display: none;"><a>';
        return $content . $custom_html;
    }
    return $content;
}
add_filter('the_title', 'insertAdBeforeContent');


// Add custom CSS + JS
function customAdsCssJs() {
    wp_register_style("customAdsStyle", plugins_url("/css/styles.css", __FILE__));
    wp_enqueue_style("customAdsStyle");

    wp_enqueue_script(
        "customAdsScript",
        plugin_dir_url(__FILE__) . "js/script.js",
        array(),
        "1.0",
        true  // Load in the footer
    );
}
add_action("wp_enqueue_scripts", "customAdsCssJs");
add_action("admin_enqueue_scripts", "customAdsCssJs");

?>
