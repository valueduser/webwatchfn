namespace WebWatcher.Nunit
{
	class TestValues
	{
		public const string bigHtmlWithNonce =
			@"<!DOCTYPE HTML>
< !--[if IE 7] >< html lang=\'en\' class=\'no-js ie7\'> <![endif]-->
<!--[if IE 8 ]><html lang =\'en\' class=\'no-js ie8\'> <![endif]-->
<!--[if IE 9 ]><html lang =\'en\' class=\'no-js ie9\'> <![endif]-->
<!--[if (gt IE 9)|!(IE)]><!-->
<html lang =\ 'en\' class=\ 'no-js\'>
<!--<![endif]-->
<html xmlns =\ 'http://www.w3.org/1999/xhtml\' lang=\ 'en-US\' prefix=\ 'og: http://ogp.me/ns#\'>

<head profile =\ 'http://gmpg.org/xfn/11\'>
    <title>My Sick Web Page.biz.co.uk.jp.cn</title>
    <meta name =\ 'description\' content=\ 'tessssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssst!\'/>
    <meta property =\ 'og:locale\' content=\ 'en_US\' />
    <meta property =\ 'og:type\' content=\ 'article\' />
    <meta property =\ 'og:title\' content=\ 'tessssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssst\'/>
    <script type = 'application/ld+json' >{\'@context\':\'http:\\/\\/schema.org\',\'@type\':\'WebSite\',\'@id\':\'#website\',\'url\':\'https:\\/\\/MyWebPage.com\\/\',\'name\':\'My Web Page Is Great\',\'potentialAction\':{\'@type\':\'SearchAction\',\'target\':\'https:\\/\\/MyWebPage.com\\/?s={search_term_string}\',\'query-input\':\'required name=search_term_string\'}}</script>
< link rel = 'dns-prefetch' href = '//ajax.googleapis.com' />

< link rel = 'dns-prefetch' href = '//cdnjs.cloudflare.com' />

< link rel = 'dns-prefetch' href = '//maxcdn.bootstrapcdn.com' />

< link rel = 'dns-prefetch' href = '//fonts.googleapis.com' />

< link rel = 'dns-prefetch' href = '//s.w.org' />

< style type =\ 'text/css\'>

		img.wp - smiley,
        img.emoji {
				display: inline!important;
				border: none!important;
				box - shadow: none!important;
				height: 1em!important;
				width: 1em!important;
				margin: 0\n.07em!important;
				vertical - align: -0.1em!important;
				background: none!important;
				padding: 0\n!important

		}

	</ style >

	< script type =\ 'text/javascript\'>

		var timerStart = Date.now();

	</ script >

	< link rel = 'stylesheet' id = 'countdown_css-css' href = 'https://MyWebPage.com/wp-content/plugins/widget-countdown/includes/style/style.css' type = 'text/css' media = 'all' />
		 
			 < link rel = 'stylesheet' id = 'thickbox-css' href = 'https://MyWebPage.com/wp-includes/js/thickbox/thickbox.css' type = 'text/css' media = 'all' />
				  </ head >
				  

				  < body class=\ 'page-template\'>
    <noscript>
        <iframe src =\ 'https://www.googletagmanager.com/ns.html?id=GTM-TJ5JR3\'\nheight=\ '0\' width=\ '0\' style=\ 'display:none;visibility:hidden\'></iframe>
    </noscript>
    <div class=\ 'wrap\'>
        <div class=\ 'contentarea-main\'>
            <div class=\ 'container\'></div>
        </div>
    </div>
    <script type = 'text/javascript' src='https://cdnjs.cloudflare.com/ajax/libs/jquery-migrate/1.4.1/jquery-migrate.min.js?ver=1.4.1'></script>
    <script type = 'text/javascript' >
		nvar the_ajax_script = {\
                'ajaxurl\':\'https:\\/\\/MyWebPage.com\\/wp-admin\\/admin-ajax.php\',\'postnonce\':\'996f8ce85a\'};\n/*  */
    </script>
    <script type = 'text/javascript' >
		nvar simcal_default_calendar = {\
                'ajax_url\':\'\\/wp-admin\\/admin-ajax.php\',\'nonce\':\'f45badb6c3\',\'locale\':\'en_US\',\'text_dir\':\'ltr\',\'months\':{\'full\':[\'January\',\'February\',\'March\',\'April\',\'May\',\'June\',\'July\',\'August\',\'September\',\'October\',\'November\',\'December\'],\'short\':[\'Jan\',\'Feb\',\'Mar\',\'Apr\',\'May\',\'Jun\',\'Jul\',\'Aug\',\'Sep\',\'Oct\',\'Nov\',\'Dec\']},\'days\':{\'full\':[\'Sunday\',\'Monday\',\'Tuesday\',\'Wednesday\',\'Thursday\',\'Friday\',\'Saturday\'],\'short\':[\'Sun\',\'Mon\',\'Tue\',\'Wed\',\'Thu\',\'Fri\',\'Sat\']},\'meridiem\':{\'AM\':\'AM\',\'am\':\'am\',\'PM\':\'PM\',\'pm\':\'pm\'}};\nvar simcal_default_calendar = {\'ajax_url\':\'\\/wp-admin\\/admin-ajax.php\',\'nonce\':\'f45badb6c3\',\'locale\':\'en_US\',\'text_dir\':\'ltr\',\'months\':{\'full\':[\'January\',\'February\',\'March\',\'April\',\'May\',\'June\',\'July\',\'August\',\'September\',\'October\',\'November\',\'December\'],\'short\':[\'Jan\',\'Feb\',\'Mar\',\'Apr\',\'May\',\'Jun\',\'Jul\',\'Aug\',\'Sep\',\'Oct\',\'Nov\',\'Dec\']},\'days\':{\'full\':[\'Sunday\',\'Monday\',\'Tuesday\',\'Wednesday\',\'Thursday\',\'Friday\',\'Saturday\'],\'short\':[\'Sun\',\'Mon\',\'Tue\',\'Wed\',\'Thu\',\'Fri\',\'Sat\']},\'meridiem\':{\'AM\':\'AM\',\'am\':\'am\',\'PM\':\'PM\',\'pm\':\'pm\'}};\n/*  */
    </script>
</body>

</html>";

		public const string bigHtmlWithRealDifference =
			@"<!DOCTYPE HTML>
< !--[if IE 7] >< html lang=\'en\' class=\'no-js ie7\'> <![endif]-->
<!--[if IE 8 ]><html lang =\'en\' class=\'no-js ie8\'> <![endif]-->
<!--[if IE 9 ]><html lang =\'en\' class=\'no-js ie9\'> <![endif]-->
<!--[if (gt IE 9)|!(IE)]><!-->
<html lang =\ 'en\' class=\ 'no-js\'>
<!--<![endif]-->
<html xmlns =\ 'http://www.w3.org/1999/xhtml\' lang=\ 'en-US\' prefix=\ 'og: http://ogp.me/ns#\'>

<head profile =\ 'http://gmpg.org/xfn/11\'>
    <title>My Sick Web Page.biz.co.uk.jp.cn</title>
    <meta name =\ 'description\' content=\ 'tessssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssst!\'/>
    <meta property =\ 'og:locale\' content=\ 'en_US\' />
    <meta property =\ 'og:type\' content=\ 'article\' />
    <meta property =\ 'og:title\' content=\ 'tessssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssst\'/>
    <script type = 'application/ld+json' >{\'@context\':\'http:\\/\\/schema.org\',\'@type\':\'WebSite\',\'@id\':\'#website\',\'url\':\'https:\\/\\/MyWebPage.com\\/\',\'name\':\'My Web Page Is Less Than Wonderful\',\'potentialAction\':{\'@type\':\'SearchAction\',\'target\':\'https:\\/\\/MyWebPage.com\\/?s={search_term_string}\',\'query-input\':\'required name=search_term_string\'}}</script>
< link rel = 'dns-prefetch' href = '//ajax.googleapis.com' />

< link rel = 'dns-prefetch' href = '//cdnjs.cloudflare.com' />

< link rel = 'dns-prefetch' href = '//maxcdn.bootstrapcdn.com' />

< link rel = 'dns-prefetch' href = '//fonts.googleapis.com' />

< link rel = 'dns-prefetch' href = '//s.w.org' />

< style type =\ 'text/css\'>

		img.wp - smiley,
        img.emoji {
				display: inline!important;
				border: none!important;
				box - shadow: none!important;
				height: 1em!important;
				width: 1em!important;
				margin: 0\n.07em!important;
				vertical - align: -0.1em!important;
				background: none!important;
				padding: 0\n!important

		}

	</ style >

	< script type =\ 'text/javascript\'>

		var timerStart = Date.now();

	</ script >

	< link rel = 'stylesheet' id = 'countdown_css-css' href = 'https://MyWebPage.com/wp-content/plugins/widget-countdown/includes/style/style.css' type = 'text/css' media = 'all' />
		 
			 < link rel = 'stylesheet' id = 'thickbox-css' href = 'https://MyWebPage.com/wp-includes/js/thickbox/thickbox.css' type = 'text/css' media = 'all' />
				  </ head >
				  

				  < body class=\ 'page-template\'>
    <noscript>
        <iframe src =\ 'https://www.googletagmanager.com/ns.html?id=GTM-TJ5JR3\'\nheight=\ '0\' width=\ '0\' style=\ 'display:none;visibility:hidden\'></iframe>
    </noscript>
    <div class=\ 'wrap\'>
        <div class=\ 'contentarea-main\'>
            <div class=\ 'container\'></div>
        </div>
    </div>
    <script type = 'text/javascript' src='https://cdnjs.cloudflare.com/ajax/libs/jquery-migrate/1.4.1/jquery-migrate.min.js?ver=1.4.1'></script>
    <script type = 'text/javascript' >
		nvar the_ajax_script = {\
                'ajaxurl\':\'https:\\/\\/MyWebPage.com\\/wp-admin\\/admin-ajax.php\',\'postnonce\':\'996f8ce85a\'};\n/*  */
    </script>
    <script type = 'text/javascript' >
		nvar simcal_default_calendar = {\
                'ajax_url\':\'\\/wp-admin\\/admin-ajax.php\',\'nonce\':\'f45badb6c3\',\'locale\':\'en_US\',\'text_dir\':\'ltr\',\'months\':{\'full\':[\'January\',\'February\',\'March\',\'April\',\'May\',\'June\',\'July\',\'August\',\'September\',\'October\',\'November\',\'December\'],\'short\':[\'Jan\',\'Feb\',\'Mar\',\'Apr\',\'May\',\'Jun\',\'Jul\',\'Aug\',\'Sep\',\'Oct\',\'Nov\',\'Dec\']},\'days\':{\'full\':[\'Sunday\',\'Monday\',\'Tuesday\',\'Wednesday\',\'Thursday\',\'Friday\',\'Saturday\'],\'short\':[\'Sun\',\'Mon\',\'Tue\',\'Wed\',\'Thu\',\'Fri\',\'Sat\']},\'meridiem\':{\'AM\':\'AM\',\'am\':\'am\',\'PM\':\'PM\',\'pm\':\'pm\'}};\nvar simcal_default_calendar = {\'ajax_url\':\'\\/wp-admin\\/admin-ajax.php\',\'nonce\':\'f45badb6c3\',\'locale\':\'en_US\',\'text_dir\':\'ltr\',\'months\':{\'full\':[\'January\',\'February\',\'March\',\'April\',\'May\',\'June\',\'July\',\'August\',\'September\',\'October\',\'November\',\'December\'],\'short\':[\'Jan\',\'Feb\',\'Mar\',\'Apr\',\'May\',\'Jun\',\'Jul\',\'Aug\',\'Sep\',\'Oct\',\'Nov\',\'Dec\']},\'days\':{\'full\':[\'Sunday\',\'Monday\',\'Tuesday\',\'Wednesday\',\'Thursday\',\'Friday\',\'Saturday\'],\'short\':[\'Sun\',\'Mon\',\'Tue\',\'Wed\',\'Thu\',\'Fri\',\'Sat\']},\'meridiem\':{\'AM\':\'AM\',\'am\':\'am\',\'PM\':\'PM\',\'pm\':\'pm\'}};\n/*  */
    </script>
</body>

</html>";

		public const string bigHtmlWithDifferentNonce =
			@"<!DOCTYPE HTML>
< !--[if IE 7] >< html lang=\'en\' class=\'no-js ie7\'> <![endif]-->
<!--[if IE 8 ]><html lang =\'en\' class=\'no-js ie8\'> <![endif]-->
<!--[if IE 9 ]><html lang =\'en\' class=\'no-js ie9\'> <![endif]-->
<!--[if (gt IE 9)|!(IE)]><!-->
<html lang =\ 'en\' class=\ 'no-js\'>
<!--<![endif]-->
<html xmlns =\ 'http://www.w3.org/1999/xhtml\' lang=\ 'en-US\' prefix=\ 'og: http://ogp.me/ns#\'>

<head profile =\ 'http://gmpg.org/xfn/11\'>
    <title>My Sick Web Page.biz.co.uk.jp.cn</title>
    <meta name =\ 'description\' content=\ 'tessssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssst!\'/>
    <meta property =\ 'og:locale\' content=\ 'en_US\' />
    <meta property =\ 'og:type\' content=\ 'article\' />
    <meta property =\ 'og:title\' content=\ 'tessssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssst\'/>
    <script type = 'application/ld+json' >{\'@context\':\'http:\\/\\/schema.org\',\'@type\':\'WebSite\',\'@id\':\'#website\',\'url\':\'https:\\/\\/MyWebPage.com\\/\',\'name\':\'My Web Page Is Great\',\'potentialAction\':{\'@type\':\'SearchAction\',\'target\':\'https:\\/\\/MyWebPage.com\\/?s={search_term_string}\',\'query-input\':\'required name=search_term_string\'}}</script>
< link rel = 'dns-prefetch' href = '//ajax.googleapis.com' />

< link rel = 'dns-prefetch' href = '//cdnjs.cloudflare.com' />

< link rel = 'dns-prefetch' href = '//maxcdn.bootstrapcdn.com' />

< link rel = 'dns-prefetch' href = '//fonts.googleapis.com' />

< link rel = 'dns-prefetch' href = '//s.w.org' />

< style type =\ 'text/css\'>

		img.wp - smiley,
        img.emoji {
				display: inline!important;
				border: none!important;
				box - shadow: none!important;
				height: 1em!important;
				width: 1em!important;
				margin: 0\n.07em!important;
				vertical - align: -0.1em!important;
				background: none!important;
				padding: 0\n!important

		}

	</ style >

	< script type =\ 'text/javascript\'>

		var timerStart = Date.now();

	</ script >

	< link rel = 'stylesheet' id = 'countdown_css-css' href = 'https://MyWebPage.com/wp-content/plugins/widget-countdown/includes/style/style.css' type = 'text/css' media = 'all' />
		 
			 < link rel = 'stylesheet' id = 'thickbox-css' href = 'https://MyWebPage.com/wp-includes/js/thickbox/thickbox.css' type = 'text/css' media = 'all' />
				  </ head >
				  

				  < body class=\ 'page-template\'>
    <noscript>
        <iframe src =\ 'https://www.googletagmanager.com/ns.html?id=GTM-TJ5JR3\'\nheight=\ '0\' width=\ '0\' style=\ 'display:none;visibility:hidden\'></iframe>
    </noscript>
    <div class=\ 'wrap\'>
        <div class=\ 'contentarea-main\'>
            <div class=\ 'container\'></div>
        </div>
    </div>
    <script type = 'text/javascript' src='https://cdnjs.cloudflare.com/ajax/libs/jquery-migrate/1.4.1/jquery-migrate.min.js?ver=1.4.1'></script>
    <script type = 'text/javascript' >
		nvar the_ajax_script = {\
                'ajaxurl\':\'https:\\/\\/MyWebPage.com\\/wp-admin\\/admin-ajax.php\',\'postnonce\':\'1234567\'};\n/*  */
    </script>
    <script type = 'text/javascript' >
		nvar simcal_default_calendar = {\
                'ajax_url\':\'\\/wp-admin\\/admin-ajax.php\',\'nonce\':\'asdfjkl\',\'locale\':\'en_US\',\'text_dir\':\'ltr\',\'months\':{\'full\':[\'January\',\'February\',\'March\',\'April\',\'May\',\'June\',\'July\',\'August\',\'September\',\'October\',\'November\',\'December\'],\'short\':[\'Jan\',\'Feb\',\'Mar\',\'Apr\',\'May\',\'Jun\',\'Jul\',\'Aug\',\'Sep\',\'Oct\',\'Nov\',\'Dec\']},\'days\':{\'full\':[\'Sunday\',\'Monday\',\'Tuesday\',\'Wednesday\',\'Thursday\',\'Friday\',\'Saturday\'],\'short\':[\'Sun\',\'Mon\',\'Tue\',\'Wed\',\'Thu\',\'Fri\',\'Sat\']},\'meridiem\':{\'AM\':\'AM\',\'am\':\'am\',\'PM\':\'PM\',\'pm\':\'pm\'}};\nvar simcal_default_calendar = {\'ajax_url\':\'\\/wp-admin\\/admin-ajax.php\',\'nonce\':\'asdfjkl\',\'locale\':\'en_US\',\'text_dir\':\'ltr\',\'months\':{\'full\':[\'January\',\'February\',\'March\',\'April\',\'May\',\'June\',\'July\',\'August\',\'September\',\'October\',\'November\',\'December\'],\'short\':[\'Jan\',\'Feb\',\'Mar\',\'Apr\',\'May\',\'Jun\',\'Jul\',\'Aug\',\'Sep\',\'Oct\',\'Nov\',\'Dec\']},\'days\':{\'full\':[\'Sunday\',\'Monday\',\'Tuesday\',\'Wednesday\',\'Thursday\',\'Friday\',\'Saturday\'],\'short\':[\'Sun\',\'Mon\',\'Tue\',\'Wed\',\'Thu\',\'Fri\',\'Sat\']},\'meridiem\':{\'AM\':\'AM\',\'am\':\'am\',\'PM\':\'PM\',\'pm\':\'pm\'}};\n/*  */
    </script>
</body>

</html>";
	}
}
