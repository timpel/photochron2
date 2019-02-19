import React, { Component } from "react";
import Dropzone from "./Dropzone";
import "./Upload.css";

class Upload extends Component {
    StatusType = Object.freeze({
        AWAITING_INPUT: Symbol("awaiting input"),
        WORKING: Symbol("working"),
        COMPLETE: Symbol("complete")
    });

    constructor(props) {
        super(props);
        this.state = {
            files: [],
            status: this.StatusType.AWAITING_INPUT,
        };

        this.onFilesAdded = this.onFilesAdded.bind(this);
        this.uploadFiles = this.uploadFiles.bind(this);
        this.renderActions = this.renderActions.bind(this);
    }

    onFilesAdded(files) {
        this.setState(prevState => ({
            files: prevState.files.concat(files)
        }));
    }

    async uploadFiles() {
        const req = new XMLHttpRequest();

        this.setState({ status: this.StatusType.WORKING });

        let files = this.state.files;
        let formData = new FormData();
        for (var i = 0; i < files.length; i++) {
            formData.append("file", files[i]);
        }
        req.responseType = "blob";
        req.open("POST", "https://localhost:44317/api/photochron/Rename");

        var that = this;

        req.onload = function () {
            var url = window.URL.createObjectURL(this.response);
            var a = document.createElement("a");
            document.body.appendChild(a);
            a.href = url;
            a.download = this.response.name || "ordered.zip"
            a.click();
            that.setState({ status: that.StatusType.COMPLETE });
        };

        req.send(formData)
    }

    renderActions() {
        if (this.state.status == this.StatusType.COMPLETE) {
            return (
                <button
                    onClick={() =>
                        this.setState({ files: [], status: this.StatusType.AWAITING_INPUT })
                    }
                >
                    Clear
        </button>
            );
        } else if (this.state.status == this.StatusType.AWAITING_INPUT) {
            return (
                <button
                    disabled={this.state.files.length < 0}
                    onClick={this.uploadFiles}
                >
                    Sort
        </button>
            );
        } else {
            // this.state.status == this.StatusType.WOKRING
            return (
                <button
                    disabled={true}
                >
                    Working...
        </button>
            );
        }
    }

    renderFiles() {
        if (this.state.status == this.StatusType.WORKING) {
            return (
                <div><p>Uploading / Renaming...</p></div>
            );
        }
        else {
            this.state.files.map(file => {
                return (
                    <div key={file.name} className="Row">
                        <span className="Filename">{file.name}</span>
                    </div>
                );
            })
        }
    }

    render() {
        return (
            <div className="Upload">
                <span className="Title">Sort Files By Date</span>
                <div className="Content">
                    <div>
                        <Dropzone
                            onFilesAdded={this.onFilesAdded}
                            disabled={this.state.status == this.StatusType.COMPLETE || this.state.status == this.StatusType.WORKING}
                        />
                    </div>
                    <div className="Files"> {
                        this.state.files.map(file => {
                            if (this.state.status != this.StatusType.WORKING) {
                                return (
                                    <div key={file.name} className="Row">
                                        <span className="Filename">{file.name}</span>
                                    </div>
                                );
                            }
                        })
                    }
                    </div>
                </div>
                <div className="Actions">{this.renderActions()}</div>
            </div>
        );
    }
}

export default Upload;