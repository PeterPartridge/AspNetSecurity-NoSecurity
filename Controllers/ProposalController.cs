﻿using AspNetSecurity_NoSecurity.Models;
using AspNetSecurity_NoSecurity.Repositories;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace AspNetSecurity_NoSecurity.Controllers
{
    public class ProposalController: Controller
    {
        private readonly ConferenceRepo conferenceRepo;
        private readonly ProposalRepo proposalRepo;
        private readonly IDataProtector protector;

        public ProposalController(ConferenceRepo conferenceRepo, ProposalRepo proposalRepo, 
            IDataProtectionProvider protectionProvider, PurposeStringConstants purposeStringConstants)
        {
            this.protector = protectionProvider.CreateProtector(purposeStringConstants.ConferenceIdQueryString);
            this.conferenceRepo = conferenceRepo;
            this.proposalRepo = proposalRepo;
        }

        public IActionResult Index(string conferenceId)
        {
            string a = protector.Unprotect(conferenceId);
            var deCryltedConferanceId = int.Parse(a);
            var conference = conferenceRepo.GetById(deCryltedConferanceId); 
            ViewBag.Title = $"Speaker - Proposals For Conference {conference.Name} {conference.Location}";
            ViewBag.ConferenceId = conferenceId;

            return View(proposalRepo.GetAllForConference(deCryltedConferanceId));
        }

        public IActionResult AddProposal(int conferenceId)
        {
            ViewBag.Title = "Speaker - Add Proposal";
            return View(new ProposalModel {ConferenceId = conferenceId});
        }
        
        [HttpPost]
        public IActionResult AddProposal(ProposalModel proposal)
        {
            if (ModelState.IsValid)
                proposalRepo.Add(proposal);
            return RedirectToAction("Index", new {conferenceId = proposal.ConferenceId});
        }

        public IActionResult Approve(int proposalId)
        {
            var proposal = proposalRepo.Approve(proposalId);
            return RedirectToAction("Index", new { conferenceId = proposal.ConferenceId });
        }
    }
}
