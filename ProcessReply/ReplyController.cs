using System.Security.Cryptography;

namespace ProcessReply
{
    public class ReplyController
    {

        #region Private Members

        IStoreClient _client = null; //存储库的客户端

        bool _enabled= false;//使能

        bool _autoplay = false; //自动播放




        DateTime _replayTime = DateTime.MinValue; //回忆时间点


        ResolutionUnit_TE _resolutionUnit = ResolutionUnit_TE.Seconde; // 时间分辨率的单位
        int _resolutionNumber = 1; //时间分辨率的数值


        IEnumerable<RecordPoint> _recordPoints = null; //记录点
        IEnumerable<QueryValue> _lastQueryValues = null; //最新的查询值

        CycleTimeTask _playTask;
        IStoreClient Clinet => _client;

        readonly Dictionary<ResolutionUnit_TE, int> _resolutionUnitMapper = new Dictionary<ResolutionUnit_TE, int>()
        {
            {ResolutionUnit_TE.Seconde,1000 },
            {ResolutionUnit_TE.Minute,60*1000 },
            {ResolutionUnit_TE.Hour,60*60*1000 },
            {ResolutionUnit_TE.Millisecond,1 },
        };
        #endregion



        #region Public Properties

        public bool TestConnect()
        {
            return _client.Ping();
        }


        /// <summary>
        /// 使能/激活
        /// </summary>
        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                _enabled = value;
                AutoPlay = false;
                if (_enabled)
                {

                    getSinglePoint(_replayTime);
                }

            }
        
        }

        /// <summary>
        /// 回放时间点
        /// </summary>
        public DateTime ReplayTime { 
            
            get {
                return _replayTime;
            } 
            
            set {

                bool _changed = _replayTime != value;
                
                _replayTime = value;

                if (_enabled)
                {

                    getSinglePoint(_replayTime);
                }

                if (_changed && ReplayTimeChanged != null)
                {
                    ReplayTimeChanged.Invoke(this, ReplayTime);
                }

            } 
        }
        /// <summary>
        /// 分辨率数值
        /// </summary>
        public int ResolutionNumber { get=>_resolutionNumber; set=>_resolutionNumber = value; }
        /// <summary>
        /// 分辨率单位
        /// </summary>
        public ResolutionUnit_TE ResolutionUnit { get=>_resolutionUnit; set=>_resolutionUnit = value; }


        /// <summary>
        /// 播放分辨率数值
        /// </summary>
        public int PlayNumber { get; set; } = 1;
        /// <summary>
        /// 播放分辨率单位
        /// </summary>
        public ResolutionUnit_TE PlayUnit { get; set; } = ResolutionUnit_TE.Seconde;



        public IEnumerable<QueryValue> LastQueryValues => _lastQueryValues;



        public bool AutoPlay
        {
            get
            {
                return _autoplay;
            }


            set
            {
                _autoplay = value;
                if (_autoplay)
                {
                    PlayStart();
                }
                else
                {
                    PlayStop();
                }
            }
        }


        #endregion


        #region Events
        /// <summary>
        /// 单点查询结束
        /// </summary>
        public event EventHandler<QueryCompletedArgs> QueryCompleted;


        public event EventHandler<DateTime> ReplayTimeChanged;

        #endregion



        /// <summary>
        /// 构建回放控制器
        /// </summary>
        /// <param name="clientType"></param>
        /// <param name="configure"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public static ReplyController Build(StoreClient_TE clientType, DataSetConfigure configure,IEnumerable<RecordPoint> points)
        {
            return new ReplyController(clientType,configure,points);
        }



        private ReplyController(StoreClient_TE clientType, DataSetConfigure configure, IEnumerable<RecordPoint> points)
        {

            switch (clientType)
            {
                case StoreClient_TE.InfluxDB:
                    _client = new InfluxDBClient(configure);
                    break;
                case StoreClient_TE.MSSQL:
                    _client = new MssqlClient(configure);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("store client type out of range");
                    break;
            }

            _recordPoints = points.ToArray();



            _playTask = new CycleTimeTask(doPlay, 1000);
            

        }






        /// <summary>
        /// 获取单点数据
        /// </summary>
        private async Task getSinglePoint(DateTime queryTime)
        {
            //await Task.Delay(1000);

            var fields = _recordPoints.Select(c => c.FieldName).ToArray();
            var results = await _client.QuerySinglePoint(fields, queryTime,this._resolutionNumber,this._resolutionUnit);

            var s = from r in results
                    join f in _recordPoints
                    on r.Name equals f.FieldName
                    select new QueryValue(r.Timestamp, f.TagName, r.Value);

            _lastQueryValues = s;

            if(QueryCompleted != null)
            {
                QueryCompleted.Invoke(this, new QueryCompletedArgs(s));
            }
        }
     


        private void PlayStart()
        {

            

            _playTask.Start();
        }


        private void PlayStop()
        {
            _playTask.Stop();
        }


        private void doPlay()
        {

            double m = 1000;
            if (_resolutionUnitMapper.TryGetValue(PlayUnit, out var u))
            {
                m = u;
            }

            var n = m * this.PlayNumber;

            ReplayTime = _replayTime.AddMilliseconds(n);

          
        }

    }
}
