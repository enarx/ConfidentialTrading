<script>
    import {onMount} from 'svelte';
    import {CrosshairMode} from 'lightweight-charts';
    import {Chart, CandlestickSeries} from 'svelte-lightweight-charts';

    let series;

    onMount(() => {
        let lastClose = data[data.length - 1].close;
        let lastIndex = data.length - 1;

        let targetIndex = lastIndex + 105 + Math.round(Math.random() + 30);
        let targetPrice = getRandomPrice();

        let currentIndex = lastIndex + 1;
        let currentBusinessDay = {day: 29, month: 5, year: 2019};
        let ticksInCurrentBar = 0;
        let currentBar = {
            open: null,
            high: null,
            low: null,
            close: null,
            time: currentBusinessDay,
        };

        const interval = setInterval(() => {
            const deltaY = targetPrice - lastClose;
            const deltaX = targetIndex - lastIndex;
            const angle = deltaY / deltaX;
            const basePrice = lastClose + (currentIndex - lastIndex) * angle;
            const noise = (0.1 - Math.random() * 0.2) + 1.0;
            const noisedPrice = basePrice * noise;
            mergeTickToBar(noisedPrice);
            if (++ticksInCurrentBar === 5) {
                // move to next bar
                currentIndex++;
                currentBusinessDay = nextBusinessDay(currentBusinessDay);
                currentBar = {
                    open: null,
                    high: null,
                    low: null,
                    close: null,
                    time: currentBusinessDay,
                };
                ticksInCurrentBar = 0;
                if (currentIndex === 5000) {
                    reset();
                    return;
                }
                if (currentIndex === targetIndex) {
                    // change trend
                    addRow(Math.round(basePrice));
                    lastClose = noisedPrice;
                    lastIndex = currentIndex;
                    targetIndex = lastIndex + 5 + Math.round(Math.random() + 30);
                    targetPrice = getRandomPrice();
                }
            }
        }, 25);

        return () => {
            clearInterval(interval);
        };

        function mergeTickToBar(price) {
            if (currentBar.open === null) {
                currentBar.open = price;
                currentBar.high = price;
                currentBar.low = price;
                currentBar.close = price;
            } else {
                currentBar.close = price;
                currentBar.high = Math.max(currentBar.high, price);
                currentBar.low = Math.min(currentBar.low, price);
            }
            series.update(currentBar);
        }

        function reset() {
            series.setData(data);
            lastClose = data[data.length - 1].close;
            lastIndex = data.length - 1;

            targetIndex = lastIndex + 5 + Math.round(Math.random() + 30);
            targetPrice = getRandomPrice();

            currentIndex = lastIndex + 1;
            currentBusinessDay = {day: 29, month: 5, year: 2019};
            ticksInCurrentBar = 0;
        }

        function getRandomPrice() {
            return 10 + Math.round(Math.random() * 10000) / 100;
        }

        function nextBusinessDay(time) {
            const d = new Date();
            d.setUTCFullYear(time.year);
            d.setUTCMonth(time.month - 1);
            d.setUTCDate(time.day + 1);
            d.setUTCHours(0, 0, 0, 0);
            return {
                year: d.getUTCFullYear(),
                month: d.getUTCMonth() + 1,
                day: d.getUTCDate(),
            };
        }
    });

    let data = [
        {time: '2019-05-29', open: 59.21, high: 59.66, low: 59.02, close: 59.57},
    ];

    let tabledata = [
        { action: "Buy", shares: "100", price: "250" },
        { action: "Sell", shares: "200", price: "350" },
    ];

	function addRow(index) {
        var buy = "Buy";
        if (Math.random() < 0.5) { buy = "Sell"; }
        const share = (Math.round(Math.random() * 5) * 50) + 50;
		tabledata = [{ action: buy, shares: share, price: index }, ...tabledata];
	}

</script>

<nav class="sidebar">
    <h1>ConfidentialTrading</h1>
    <table class="table">
    <thead>
        <tr>
            <td>Action</td>
            <td>Shares</td>
            <td>Price</td>
        </tr>
    </thead>
    <tbody>
        {#each tabledata as item}
            <tr class={item.action === "Buy" ? "buy" : "sell"}>
                <td>{item.action}</td>
                <td>{item.shares}</td>
                <td>{item.price}</td>
            </tr>
        {/each}
    </tbody>
    </table>
</nav>
<div class="main">
    <Chart
        width={1200}
        height={600}
        layout={{
            backgroundColor: '#2B2B43',
            textColor: '#D9D9D9'
        }}
        grid={{
            vertLines: '#2B2B43',
            horzLines: '#363C4E'
        }}
        crosshair={{mode: CrosshairMode.Normal}}
    >
        <CandlestickSeries
            data={data}
            reactive={true}
            ref={(api) => series = api}
        />
    </Chart>
</div>