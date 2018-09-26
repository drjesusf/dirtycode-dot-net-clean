using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer
{
    /// <summary>
    /// Represents a single speaker
    /// </summary>
    public class Speaker
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public int? YearsOfExperience { get; set; }
		public bool HasBlog { get; set; }
		public string BlogURL { get; set; }
		public WebBrowser Browser { get; set; }
		public List<string> Certifications { get; set; }
		public string Employer { get; set; }
		public int RegistrationFee { get; set; }
		public List<BusinessLayer.Session> Sessions { get; set; }

		/// <summary>
		/// Register a speaker
		/// </summary>
		/// <returns>speakerID</returns>
		public int? Register(IRepository repository)
		{
			int? speakerId = null;
			bool good = false;
			bool approvedSpeaker = false;

			var olderTech = new List<string>() { "Cobol", "Punch Cards", "Commodore", "VBScript" };
			var domains = new List<string>() { "aol.com", "hotmail.com", "prodigy.com", "CompuServe.com" };

            if(string.IsNullOrWhiteSpace(FirstName)) throw new ArgumentNullException("First Name is required");
            if (string.IsNullOrWhiteSpace(LastName)) throw new ArgumentNullException("Last name is required.");
            if (string.IsNullOrWhiteSpace(Email)) throw new ArgumentNullException("Email is required.");

			var employers = new List<string>() { "Microsoft", "Google", "Fog Creek Software", "37Signals" };
            const int minimumCertifications = 3;
            const int minimumExperiencieYears = 10;
            const int maximumMajorVersion = 9;
            good = ((YearsOfExperience > minimumExperiencieYears || HasBlog || Certifications.Count() > minimumCertifications || employers.Contains(Employer)));

			if (!good)
			{
				string onlyEmailDomain = Email.Split('@').Last();

				if (!domains.Contains(onlyEmailDomain) && (!(Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < maximumMajorVersion)))
				{
					good = true;
				}
			}
            if(!good) throw new SpeakerDoesntMeetRequirementsException("Speaker doesn't meet our abitrary and capricious standards.");

            if (!Sessions.Any()) throw new ArgumentException("Can't register speaker with no sessions to present.");
            //DEFECT #5013 CO 1/12/2012
            //We weren't requiring at least one session
            
			foreach (var session in Sessions)
			{
				foreach (var tech in olderTech)
				{
					if (session.Title.Contains(tech) || session.Description.Contains(tech))
					{
						session.Approved = false;
						break;
					}
					else
					{
						session.Approved = true;
						approvedSpeaker = true;
					}
				}
			}
			
            if (!approvedSpeaker) throw new NoSessionsApprovedException("No sessions approved.");
            
			if (YearsOfExperience <= 1)
			{
				RegistrationFee = 500;
			}
			else if (YearsOfExperience >= 2 && YearsOfExperience <= 3)
			{
				RegistrationFee = 250;
			}
			else if (YearsOfExperience >= 4 && YearsOfExperience <= 5)
			{
				RegistrationFee = 100;
			}
			else if (YearsOfExperience >= 6 && YearsOfExperience <= 9)
			{
				RegistrationFee = 50;
			}
			else
			{
				RegistrationFee = 0;
			}

			try
			{
				speakerId = repository.SaveSpeaker(this);
			}
			catch (Exception e)
			{
				//in case the db call fails 
			}

			return speakerId;
		}

		#region Custom Exceptions
		public class SpeakerDoesntMeetRequirementsException : Exception
		{
			public SpeakerDoesntMeetRequirementsException(string message)
				: base(message)
			{
			}

			public SpeakerDoesntMeetRequirementsException(string format, params object[] args)
				: base(string.Format(format, args)) { }
		}

		public class NoSessionsApprovedException : Exception
		{
			public NoSessionsApprovedException(string message)
				: base(message)
			{
			}
		}
		#endregion
	}
}