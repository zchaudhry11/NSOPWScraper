# NSOPWScraper

This is a scraper that is used to gather all of the data from the National Sex Offender Public Website, a database containing various pieces of information about sex offenders within the USA. Due to the current layout of their website, scraping the data directly was not possible. In order for this scraper to be effective, you should scrape the HTML pages from the NSOPW and then pass those files to this scraper.

The output will be a set of .csv files containing all of the information related to each offender with the first column containing column names. The NSOPW
database has a number of anomolies located within their data set however and as a result you will want to double check
your output and clean the anomalies.

Some example anomalies include:
<ul>
<li>Offenders with no name</li>
<li>Names that begin with "ALIAS: "</li>
<li>Values named "Null"</li>
<li>A comma character ',' taking the place of an offender field</li>
<li>Periods located within entries "123. FAKE. STREET."</li>
</ul>
