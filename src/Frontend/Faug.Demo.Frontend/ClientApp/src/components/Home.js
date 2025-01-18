import React, { Component } from 'react';
import { NavLink, Link } from "react-router-dom";
export class Home extends Component {
    // static displayName = FetchData.name;

    constructor(props) {
        super(props);
        this.state = { locations: [], loading: true };
    }

    componentDidMount() {
        this.populateLocationData();
    }

    static renderLocationsTable(locations) {
        return (
            <table className='table table-striped' aria-labelledby="tabelLabel">
                <thead>
                    <tr>
                        <th>City</th>
                        <th>Lat</th>
                        <th>Lng</th>
                        <th>Population</th>
                    </tr>
                </thead>
                <tbody>
                    {locations.map(location =>
                        <tr key={location.city + "_" + location.lat + "_" + location.lng}>
                            <td><Link to={`/locations?lat=${location.lat}&lng=${location.lng}&city=${location.city}`}>{location.city}</Link></td>
                            <td>{location.lat}</td>
                            <td>{location.lng}</td>
                            <td>{location.population}</td>
                        </tr>
                    )}
                </tbody>
            </table>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : Home.renderLocationsTable(this.state.locations);

        return (
            <div>
                <h1 id="tabelLabel">Locations</h1>
                {contents}
            </div>
        );
    }

    async populateLocationData() {
        const response = await fetch('/api/locations/fi');
        const data = await response.json();
        this.setState({ locations: data, loading: false });
    }
}
