using System;

namespace UpbitAPI_CS_Wrapper
{
    class Program
    {
        static void Main(string[] args)
        {
            UpbitAPI U = new UpbitAPI("accessKey 입력", "secretKey 입력");

            #region 자산
            // 자산 조회
            Console.WriteLine(U.GetAccount());
            #endregion

            #region 주문
            // 주문 가능 정보
            Console.WriteLine(U.GetOrderChance("KRW-BTC"));

            // 개별 주문 조회
            Console.WriteLine(U.GetOrder("주문 uuid"));

            // 주문 리스트 조회
            Console.WriteLine(U.GetAllOrder());

            // 주문하기
            Console.WriteLine(U.MakeOrder("KRW-BTC", UpbitAPI.UpbitOrderSide.bid, 0.001m, 5000000));

            // 주문 취소
            Console.WriteLine(U.CancelOrder("주문 uuid"));
            #endregion

            #region 시세 정보
            // 마켓 코드 조회
            Console.WriteLine(U.GetMarkets());

            // 캔들(분, 일, 주, 월) 조회
            Console.WriteLine(U.GetCandles_Minute("KRW-BTC", UpbitAPI.UpbitMinuteCandleType._1, to: DateTime.Now.AddMinutes(-2), count: 2));
            Console.WriteLine(U.GetCandles_Day("KRW-BTC", to: DateTime.Now.AddDays(-2), count: 2));
            Console.WriteLine(U.GetCandles_Week("KRW-BTC", to: DateTime.Now.AddDays(-14), count: 2));
            Console.WriteLine(U.GetCandles_Month("KRW-BTC", to: DateTime.Now.AddMonths(-2), count: 2));

            // 당일 체결 내역 조회
            Console.WriteLine(U.GetTicks("KRW-BTC", count: 2));

            // 현재가 정보 조회
            Console.WriteLine(U.GetTicker("KRW-BTC,KRW-ETH"));
            
            // 시세 호가 정보(Orderbook) 조회
            Console.WriteLine(U.GetOrderbook("KRW-BTC,KRW-ETH"));
            #endregion

            Console.ReadLine();
        }
    }
}
