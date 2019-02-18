import React, { Component } from 'react';
import "./Rename.css";
import Upload from "./Upload.js";

export class Rename extends Component {
    displayName = Rename.name

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div className="App">
                <div className="Card">
                    <Upload />
                </div>
            </div>
        );
    }
}
