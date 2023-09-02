using System;
using SNPM.Core;
using SNPM.Core.Interfaces;
using SNPM.MVVM.Models;
using Moq;
using SNPM.Core.Interfaces.Api;

namespace SNPM.Test.MVVM.Models
{
    [TestClass]
    public class PasswordVerifierTest
    {
        private Task<bool> remoteVerifierMock(string password) => Task.FromResult(true);
        private IPasswordVerifier PasswordVerifierService;

        [TestInitialize]
        public void Init()
        {
            var mockApiService = new Mock<IApiService>();
            mockApiService.Setup(x => x.GetRemoteVerifier()).Returns(remoteVerifierMock);

            PasswordVerifierService = new PasswordVerifier(mockApiService.Object);
        }

        [DataTestMethod]
        [DataRow("Password", 5, false)]
        [DataRow("Password", 1, false)]
        [DataRow("", 5, true)]
        [DataRow("", 0, false)]
        [DataRow("ABCDEFGHIJKLMNOUPRSTV", 21, false)]
        [DataRow("a", 2147483647, true)]
        [DataRow("this is a password with spaces", 412, true)]
        [DataRow("justAPassword123", 20, true)]
        public void GivenPassword_VerifyLength(string password, int length, bool expected)
        {
            // Arrange
            PasswordVerifierService.PasswordPolicy = new PasswordPolicy(length, false, PasswordQuality.InvalidLength, CharacterGroup.None);

            // Act
            var verificationTask = PasswordVerifierService.VerifyPassword(password);
            verificationTask.Wait();
            var result = verificationTask.Result;

            // Assert
            Assert.AreEqual(result.HasFlag(PasswordQuality.InvalidLength), expected);
        }

        [DataTestMethod]
        [DataRow("password", CharacterGroup.Lowercase, false)]
        [DataRow("password", CharacterGroup.Lowercase | CharacterGroup.Uppercase, true)]
        [DataRow("Password", CharacterGroup.Lowercase | CharacterGroup.Uppercase, false)]
        [DataRow("Password", CharacterGroup.Lowercase | CharacterGroup.Uppercase | CharacterGroup.Numeric, true)]
        [DataRow("Password1", CharacterGroup.Lowercase | CharacterGroup.Uppercase | CharacterGroup.Numeric, false)]
        [DataRow("Password1", CharacterGroup.Lowercase | CharacterGroup.Uppercase | CharacterGroup.Numeric | CharacterGroup.Special, true)]
        [DataRow("Password1@", CharacterGroup.Lowercase | CharacterGroup.Uppercase | CharacterGroup.Numeric | CharacterGroup.Special, false)]
        [DataRow("Password1🏳️‍🌈", CharacterGroup.Lowercase | CharacterGroup.Uppercase | CharacterGroup.Numeric | CharacterGroup.Special, false)]
        [DataRow("Password1🏳️‍🌈", CharacterGroup.None, false)]
        [DataRow("", CharacterGroup.None, false)]
        [DataRow("🏳️‍🌈", CharacterGroup.Lowercase, true)]
        [DataRow("🏳️‍🌈", CharacterGroup.Uppercase, true)]
        [DataRow("🏳️‍🌈", CharacterGroup.Numeric, true)]
        [DataRow("🏳️‍🌈", CharacterGroup.Special, false)]
        [DataRow(" ", CharacterGroup.Special, false)]
        [DataRow("\n", CharacterGroup.Special, false)]
        public void GivenPassword_VerifyCharacterGroups(string password, CharacterGroup requiredGroups, bool expected)
        {
            // Arrange
            PasswordVerifierService.PasswordPolicy = new PasswordPolicy(0, false, PasswordQuality.NotEnoughWordGroups, requiredGroups);

            // Act
            var verificationTask = PasswordVerifierService.VerifyPassword(password);
            verificationTask.Wait();
            var result = verificationTask.Result;

            // Assert
            Assert.AreEqual(result.HasFlag(PasswordQuality.NotEnoughWordGroups), expected);
        }
    }
}
