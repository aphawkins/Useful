// <copyright file="CipherRepositoryTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Useful.UnitTests
{
    using System;
    using Moq;
    using Useful.Security.Cryptography;
    using Xunit;

    public class CipherRepositoryTests : IDisposable
    {
        private CipherRepository _repository;
        private Mock<ICipher> _moqCipher;

        public CipherRepositoryTests()
        {
            _repository = new CipherRepository();
            _moqCipher = new Mock<ICipher>();
            _moqCipher.Setup(x => x.CipherName).Returns("MoqCipherName");
        }

        [Fact]
        public void RepositoryCreate()
        {
            int count = _repository.Read().Count;
            _repository.Create(_moqCipher.Object);
            Assert.Equal(count + 1, _repository.Read().Count);
        }

        [Fact]
        public void RepositoryRead()
        {
            Assert.Equal(3, _repository.Read().Count);
        }

        [Fact]
        public void RepositoryUpdate()
        {
            int count = _repository.Read().Count;
            _repository.Update(_moqCipher.Object);
            Assert.Equal(count, _repository.Read().Count);
        }

        [Fact]
        public void RepositoryDelete()
        {
            int count = _repository.Read().Count;
            _repository.Delete(_repository.Read()[0]);
            Assert.Equal(count - 1, _repository.Read().Count);
        }

        [Fact]
        public void RepositorySetCurrentItem()
        {
            _repository.Create(_moqCipher.Object);
            _repository.SetCurrentItem(x => x.CipherName == "MoqCipherName");
            Assert.Equal(_repository.CurrentItem, _moqCipher.Object);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                _repository = null;
                _moqCipher = null;
            }

            // free native resources if there are any.
        }
    }
}