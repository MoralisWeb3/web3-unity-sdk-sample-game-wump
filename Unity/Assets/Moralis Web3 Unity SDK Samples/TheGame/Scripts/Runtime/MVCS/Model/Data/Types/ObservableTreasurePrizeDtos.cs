
using System;
using System.Collections.Generic;
using MoralisUnity.Samples.Shared.Data.Types;
using UnityEngine;

namespace MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types
{
    /// <summary>
    /// Observable wrapper invoking events for <see cref="MoralisUnity.Samples.TheGame.MVCS.Model.Data.Types.PrizeDto"/>
    /// </summary>
    [Serializable]
    public class ObservableTreasurePrizeDtos : Observable<List<TreasurePrizeDto>>
    {
        //  Properties ------------------------------------


        //  Fields ----------------------------------------

        //  Constructor Methods ---------------------------
        public ObservableTreasurePrizeDtos()
        {
            Value = new List<TreasurePrizeDto>();
        }

        //  General Methods -------------------------------


        //  Event Handlers --------------------------------

    }

}
