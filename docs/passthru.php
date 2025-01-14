<?php

    $dbhost = 'localhost'; 
    $dbuser = 'graysone_banker'; 
    $dbpass = 'bankersgotohell'; 
    $dbname = 'graysone_launchabanker'; 
    $dbtable = 'highscore'; 

    $dbh = mysql_connect($dbhost, $dbuser, $dbpass);
    if(!$dbh)  {
        die("Unable to connect to DB: " . mysql_error());
    }

    $db = mysql_select_db($dbname);

    if (!$db)  {
        die("Use database failed: " . mysql_error());
    }

    $bad_name = $_GET['name'];

    $bad_score = $_GET['score'];

    $good_score = $bad_score;

    if($bad_name == '') { die("Empty!"); }

    $good_name = sanitize($bad_name);


    echo $good_name;
    echo "<br>";
    echo $good_score;


    $q = "INSERT INTO $dbtable (score, playername) VALUES('" . $good_name . "', $good_score);

    $r = mysql_query($q);


    // clean!
    function cleanInput($input) {
        $search = array(
        '@<script[^>]*?>.*?</script>@si',   // Strip out javascript
        '@<[\/\!]*?[^<>]*?>@si',            // Strip out HTML tags
        '@<style[^>]*?>.*?</style>@siU',    // Strip style tags properly
        '@<![\s\S]*?--[ \t\n\r]*>@'         // Strip multi-line comments
    );
 
    $output = preg_replace($search, '', $input);
    return $output;
  }

    // sanitize!
    function sanitize($input) {
        if (is_array($input)) {
            foreach($input as $var=>$val) {
                $output[$var] = sanitize($val);
            }
        }
        else {
            if (get_magic_quotes_gpc()) {
                $input = stripslashes($input);
            }
            $input  = cleanInput($input);
            $output = mysql_real_escape_string($input);
        }
        return $output;
    }


?>