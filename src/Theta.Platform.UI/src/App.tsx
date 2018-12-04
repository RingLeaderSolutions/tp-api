import React, { Component } from 'react';
import logo from './logo.png';
import './App.css';
import { HubConnectionBuilder, HubConnection, LogLevel, IHttpConnectionOptions, HttpError } from "@aspnet/signalr";
import axios from 'axios';

let connectionOptions: IHttpConnectionOptions = {
    // logger: new PrefixedConsoleLogger(logLevel, 'SignalR'),
    // logMessageContent: logVerbose,
    // accessTokenFactory: () => this.storage.fetchIdToken()
};

let connection = new HubConnectionBuilder()
    .withUrl("https://localhost:15102/hub", connectionOptions)
    .configureLogging(LogLevel.Debug)
    .build();

connection.onclose((error?: Error) => console.error(error))

connection.start();

interface Instrument {
    id: string;
    name: string;
    category: string;
    ids: InstrumentIdentifier[];
    couponRate: number;
}

interface InstrumentIdentifier {
    id: string;
    identifierType: string;
}

interface AppState {
    loaded: boolean;
    instruments: Instrument[]
}

class App extends Component<void, AppState> {
    constructor() {
        super();
        this.state = {
            loaded: false,
            instruments: []
        }
    }

    componentDidMount(){
        axios.get("http://localhost:15105/api/Instrument")
            .then(ar => {
                let instruments = ar.data;
                this.setState({
                    loaded: true,
                    instruments
                })
            })
    }
    
    subscribeToInstrument(){
        connection.invoke("subscribe", "GB00B16NNR78");
    }

    unsubscribeFromInstrument(){
        connection.invoke("unsubscribe", "GB00B16NNR78");
    }

    renderInstrumentOptions(){
        return this.state.instruments.map(i => {
            return (<option key={i.id} value={i.id}>{i.category} {i.name} - {i.couponRate}</option>)
        })
    }

    render() {
        if(!this.state.loaded){
            return <div className="App"><p>Loading Instrument Data...</p></div>;
        }

        return (
        <div className="App">
            <header className="App-header">
            <img src={logo} className="App-logo" alt="logo"/>
            <p>Hello!</p>
            <select>
                <option>Select Instrument</option>
                {this.renderInstrumentOptions()}
            </select>
            <button onClick={() => this.subscribeToInstrument()}>Click me to subscribe!</button>
            <button onClick={() => this.unsubscribeFromInstrument()}>Click me to unsubscribe!</button>
            </header>
        </div>
        );
    }
}

export default App;
