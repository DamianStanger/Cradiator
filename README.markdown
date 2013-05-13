Cradiator's issue management is still at <http://cradiator.codeplex.com/> (wiki, discussion)  
Cradiator now lives on GitHub (source code)

## Contributors

* [RubenWillems](http://www.codeplex.com/site/users/view/RubenWillems)
* [Craig Sutherland](http://www.codeplex.com/site/users/view/csut017)

### Config documentation (serving as a rough functionality guide)

* Polling Frequency - In seconds. Default is 30

* views/view - Each 'view' can be defined in this xml section. 
- If more than 1 view is specified, then the view is switched (on a rotation cycle) at each poll interval.
- Each view contains a url & other base settings as documented below
	
* view/url - The (xml) status report URL
- eg for CCNet - http://ccnetlive.thoughtworks.com/ccnet/XmlStatusReport.aspx
- eg for Java  - http://www.spice-3d.org/cruise/xml
- eg for Ruby (.rb) http://cruisecontrolrb.thoughtworks.com/XmlStatusReport.aspx (If URL ends with 'ccnet' "XmlStatusReport.aspx" will be auto-appended)
[debug-mode] - prepend the URL with the word 'debug' - this switches to using an xml file in the bin folder named 'DummyProjectStatus.xml' instead of connecting to the WebService - useful for testing /screenshots etc
		 
[multi-url] - URL can be split (using space as a delimiter), to refer to multiple urls. 
- eg value="http://url1 http://url2"
- All project data is collected into one screen output 
- DEBUG MODE overrides the multiurl feature; no multiurls are read if debug
	
* view/project-regex - RegEx used to filter which projects are included (by name)
- Defaults to ".*" (even if config = "")
	
* view/category-regex - as for ProjectNameRegEx but filters by category name

* view/server-regex - as for ProjectNameRegEx but filters by server name 
- for ccnet this is the name of the build server as defined in dashboard.config in dashboard\remoteServices\servers\server

* view/name - the name for this view, will be shown when ShowOnlyBroken = true and there are no broken projects

* view/showServerName - shows the servername below the project name if true, handy if you monitor multiple servers with identical project names


* view/skin - Currently 3 choices 
(1) Grid - arranged in a grid format
(2) Stack - arranged in a stack (ie top-to-bottom listbox type) format
(3) StackPhoto - same as Stack but shows an image of the build breaker as well as text 
- last/first listed - is the last/first person to commit while build is broken - where 'last/first' 
is dependent on the setting 'BreakerGuiltStrategy' (below)


[photo/image] functionality, requires a sub-folder named 'images' (relative to folder in which
the Cradiator.exe resides) with a JPEG ([username].jpg) corresponding to each username (auto-created by the installer)
 - The JPEG file must be named using the username - eg bsimpson requires a filepath 'images/bsimpson.jpg'
 - If a file/photo does not exist for a user, everything will still work as normal (ie it's not considered an error)
	
* ShowCountdown - 'true' or 'false' (case insensitive) ,so { True False TRUE FALSE } are all valid
- Shows a clock that counts down the 'time to go' before refreshing the screen (updated approx every second)
	
* PlaySounds - true/false, whether to play sounds on events (described below) (.WAV files only)
* PlaySpeech - true/false, whether to speak (ie use SpeechSynthesizer) on certain build events - is configurable (see below)  

* BrokenBuildSound - the filename (without path) of the .WAV file
- The file is assumed to be in a sub-folder named 'sounds' (relative to the folder in which Cradiator.exe resides) 
- A 'BrokenBuildSound' plays in response to a project that starts 1) not Broken (FAILURE|EXCEPTION) followed by 2) Broken  
	
* FixedBuildSound - as for BrokenBuildSound, but plays in the case of 1) Broken (FAILURE|EXCEPTION) followed by 2) SUCCESS

* BrokenBuildText - text can be just text or include variables which are replaced at runtime
[Plain Text] - Voice automatically speaks the project name, followed by configured text eg voice speaks "Project1 [your text goes here]." 
 - Default value is "is broken"
[Custom] - Can include 2 variables in your text (1) $ProjectName$ (2) $Breaker$ (only for broken builds)) 
 - the variables are used to specify values that are known only at run time, for that particular project 
 -  eg "$ProjectName$ is broken, $Breaker$, you're fired!"
				
* FixedBuildText - as for BrokenBuildText - default value is "is fixed"

* SpeechVoiceName - The (SAPI) voice name used in speech synthesis.
- The voice name does not have to be exactly correct, for example 'william' is close enough to select the 'Cepstral William' voice. 
- If voice name is ambiguous then first matching name, in alphabetical order, is selected

* usernames - a section for mapping usernames to real or full-length names for use by the SpeechSynthesizer 
- used when figuring the $Breaker$ variable in Fixed/BrokenBuildText eg if your username is 'jbloggs' - this may 
not sound comprehensible when spoken by the voice synthesizer, hence you can map this username to 
'Johann Bloggs' via this config. 
- A username of 'jsmith' is mapped to 'John Smith' by default, in app.config, to make the format required obvious
					
* BreakerGuiltStrategy - 'First' or 'Last'. 
- How to determine the 'Breaker', is the 'First' build breaker always guilty?, or do subsequent committers ('Last' breaker) assume guilt when they commit over a breaking build?