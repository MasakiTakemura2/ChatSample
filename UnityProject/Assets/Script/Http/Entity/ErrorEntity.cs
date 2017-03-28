using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Http
{
    public class ErrorEntity {

        [Serializable]
        public class Result {
            public Error result;
        }

        [Serializable]
        public class Error {
            public List<string> error;
        }
	}
}
